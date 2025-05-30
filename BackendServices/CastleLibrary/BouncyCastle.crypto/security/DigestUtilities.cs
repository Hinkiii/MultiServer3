using System;
using System.Collections.Generic;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.Misc;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Rosstandart;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.UA;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Security
{
    /// <remarks>
    ///  Utility class for creating IDigest objects from their names/Oids
    /// </remarks>
    public static class DigestUtilities
    {
        private enum DigestAlgorithm {
            BLAKE2B_160, BLAKE2B_256, BLAKE2B_384, BLAKE2B_512,
            BLAKE2S_128, BLAKE2S_160, BLAKE2S_224, BLAKE2S_256,
            BLAKE3_256,
            DSTU7564_256, DSTU7564_384, DSTU7564_512,
            GOST3411,
            GOST3411_2012_256, GOST3411_2012_512,
            KECCAK_224, KECCAK_256, KECCAK_288, KECCAK_384, KECCAK_512,
            MD2, MD4, MD5,
            NONE,
            RIPEMD128, RIPEMD160, RIPEMD256, RIPEMD320,
            SHA_1, SHA_224, SHA_256, SHA_384, SHA_512,
            SHA_512_224, SHA_512_256,
            SHA3_224, SHA3_256, SHA3_384, SHA3_512,
            SHAKE128_256, SHAKE256_512,
            SM3,
            TIGER,
            WHIRLPOOL,
        };

        private static readonly Dictionary<string, string> AlgorithmMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<DerObjectIdentifier, string> AlgorithmOidMap =
            new Dictionary<DerObjectIdentifier, string>();
        private static readonly Dictionary<string, DerObjectIdentifier> Oids =
            new Dictionary<string, DerObjectIdentifier>(StringComparer.OrdinalIgnoreCase);

        static DigestUtilities()
        {
            // Signal to obfuscation tools not to change enum constants
            Enums.GetArbitraryValue<DigestAlgorithm>().ToString();

            AlgorithmOidMap[PkcsObjectIdentifiers.MD2] = "MD2";
            AlgorithmOidMap[PkcsObjectIdentifiers.MD4] = "MD4";
            AlgorithmOidMap[PkcsObjectIdentifiers.MD5] = "MD5";

            AlgorithmMap["SHA1"] = "SHA-1";
            AlgorithmOidMap[OiwObjectIdentifiers.IdSha1] = "SHA-1";
            AlgorithmOidMap[PkcsObjectIdentifiers.IdHmacWithSha1] = "SHA-1";
            AlgorithmOidMap[MiscObjectIdentifiers.HMAC_SHA1] = "SHA-1";
            AlgorithmMap["SHA224"] = "SHA-224";
            AlgorithmOidMap[NistObjectIdentifiers.IdSha224] = "SHA-224";
            AlgorithmOidMap[PkcsObjectIdentifiers.IdHmacWithSha224] = "SHA-224";
            AlgorithmMap["SHA256"] = "SHA-256";
            AlgorithmOidMap[NistObjectIdentifiers.IdSha256] = "SHA-256";
            AlgorithmOidMap[PkcsObjectIdentifiers.IdHmacWithSha256] = "SHA-256";
            AlgorithmMap["SHA384"] = "SHA-384";
            AlgorithmOidMap[NistObjectIdentifiers.IdSha384] = "SHA-384";
            AlgorithmOidMap[PkcsObjectIdentifiers.IdHmacWithSha384] = "SHA-384";
            AlgorithmMap["SHA512"] = "SHA-512";
            AlgorithmOidMap[NistObjectIdentifiers.IdSha512] = "SHA-512";
            AlgorithmOidMap[PkcsObjectIdentifiers.IdHmacWithSha512] = "SHA-512";

            AlgorithmMap["SHA512/224"] = "SHA-512/224";
            AlgorithmMap["SHA512-224"] = "SHA-512/224";
            AlgorithmMap["SHA512(224)"] = "SHA-512/224";
            AlgorithmMap["SHA-512(224)"] = "SHA-512/224";
            AlgorithmOidMap[NistObjectIdentifiers.IdSha512_224] = "SHA-512/224";
            AlgorithmOidMap[PkcsObjectIdentifiers.IdHmacWithSha512_224] = "SHA-512/224";
            AlgorithmMap["SHA512/256"] = "SHA-512/256";
            AlgorithmMap["SHA512-256"] = "SHA-512/256";
            AlgorithmMap["SHA512(256)"] = "SHA-512/256";
            AlgorithmMap["SHA-512(256)"] = "SHA-512/256";
            AlgorithmOidMap[NistObjectIdentifiers.IdSha512_256] = "SHA-512/256";
            AlgorithmOidMap[PkcsObjectIdentifiers.IdHmacWithSha512_256] = "SHA-512/256";

            AlgorithmMap["RIPEMD-128"] = "RIPEMD128";
            AlgorithmOidMap[TeleTrusTObjectIdentifiers.RipeMD128] = "RIPEMD128";
            AlgorithmMap["RIPEMD-160"] = "RIPEMD160";
            AlgorithmOidMap[TeleTrusTObjectIdentifiers.RipeMD160] = "RIPEMD160";
            AlgorithmMap["RIPEMD-256"] = "RIPEMD256";
            AlgorithmOidMap[TeleTrusTObjectIdentifiers.RipeMD256] = "RIPEMD256";
            AlgorithmMap["RIPEMD-320"] = "RIPEMD320";
            //AlgorithmOidMap[TeleTrusTObjectIdentifiers.RipeMD320] = "RIPEMD320";

            AlgorithmOidMap[CryptoProObjectIdentifiers.GostR3411] = "GOST3411";

            AlgorithmMap["KECCAK224"] = "KECCAK-224";
            AlgorithmMap["KECCAK256"] = "KECCAK-256";
            AlgorithmMap["KECCAK288"] = "KECCAK-288";
            AlgorithmMap["KECCAK384"] = "KECCAK-384";
            AlgorithmMap["KECCAK512"] = "KECCAK-512";

            AlgorithmOidMap[NistObjectIdentifiers.IdSha3_224] = "SHA3-224";
            AlgorithmOidMap[NistObjectIdentifiers.IdHMacWithSha3_224] = "SHA3-224";
            AlgorithmOidMap[NistObjectIdentifiers.IdSha3_256] = "SHA3-256";
            AlgorithmOidMap[NistObjectIdentifiers.IdHMacWithSha3_256] = "SHA3-256";
            AlgorithmOidMap[NistObjectIdentifiers.IdSha3_384] = "SHA3-384";
            AlgorithmOidMap[NistObjectIdentifiers.IdHMacWithSha3_384] = "SHA3-384";
            AlgorithmOidMap[NistObjectIdentifiers.IdSha3_512] = "SHA3-512";
            AlgorithmOidMap[NistObjectIdentifiers.IdHMacWithSha3_512] = "SHA3-512";
            AlgorithmMap["SHAKE128"] = "SHAKE128-256";
            AlgorithmOidMap[NistObjectIdentifiers.IdShake128] = "SHAKE128-256";
            AlgorithmMap["SHAKE256"] = "SHAKE256-512";
            AlgorithmOidMap[NistObjectIdentifiers.IdShake256] = "SHAKE256-512";

            AlgorithmOidMap[GMObjectIdentifiers.sm3] = "SM3";

            AlgorithmOidMap[MiscObjectIdentifiers.id_blake2b160] = "BLAKE2B-160";
            AlgorithmOidMap[MiscObjectIdentifiers.id_blake2b256] = "BLAKE2B-256";
            AlgorithmOidMap[MiscObjectIdentifiers.id_blake2b384] = "BLAKE2B-384";
            AlgorithmOidMap[MiscObjectIdentifiers.id_blake2b512] = "BLAKE2B-512";
            AlgorithmOidMap[MiscObjectIdentifiers.id_blake2s128] = "BLAKE2S-128";
            AlgorithmOidMap[MiscObjectIdentifiers.id_blake2s160] = "BLAKE2S-160";
            AlgorithmOidMap[MiscObjectIdentifiers.id_blake2s224] = "BLAKE2S-224";
            AlgorithmOidMap[MiscObjectIdentifiers.id_blake2s256] = "BLAKE2S-256";
            AlgorithmOidMap[MiscObjectIdentifiers.blake3_256] = "BLAKE3-256";

            AlgorithmOidMap[RosstandartObjectIdentifiers.id_tc26_gost_3411_12_256] = "GOST3411-2012-256";
            AlgorithmOidMap[RosstandartObjectIdentifiers.id_tc26_gost_3411_12_512] = "GOST3411-2012-512";

            AlgorithmOidMap[UAObjectIdentifiers.dstu7564digest_256] = "DSTU7564-256";
            AlgorithmOidMap[UAObjectIdentifiers.dstu7564digest_384] = "DSTU7564-384";
            AlgorithmOidMap[UAObjectIdentifiers.dstu7564digest_512] = "DSTU7564-512";

            Oids["MD2"] = PkcsObjectIdentifiers.MD2;
            Oids["MD4"] = PkcsObjectIdentifiers.MD4;
            Oids["MD5"] = PkcsObjectIdentifiers.MD5;
            Oids["SHA-1"] = OiwObjectIdentifiers.IdSha1;
            Oids["SHA-224"] = NistObjectIdentifiers.IdSha224;
            Oids["SHA-256"] = NistObjectIdentifiers.IdSha256;
            Oids["SHA-384"] = NistObjectIdentifiers.IdSha384;
            Oids["SHA-512"] = NistObjectIdentifiers.IdSha512;
            Oids["SHA-512/224"] = NistObjectIdentifiers.IdSha512_224;
            Oids["SHA-512/256"] = NistObjectIdentifiers.IdSha512_256;
            Oids["SHA3-224"] = NistObjectIdentifiers.IdSha3_224;
            Oids["SHA3-256"] = NistObjectIdentifiers.IdSha3_256;
            Oids["SHA3-384"] = NistObjectIdentifiers.IdSha3_384;
            Oids["SHA3-512"] = NistObjectIdentifiers.IdSha3_512;
            Oids["SHAKE128-256"] = NistObjectIdentifiers.IdShake128;
            Oids["SHAKE256-512"] = NistObjectIdentifiers.IdShake256;
            Oids["RIPEMD128"] = TeleTrusTObjectIdentifiers.RipeMD128;
            Oids["RIPEMD160"] = TeleTrusTObjectIdentifiers.RipeMD160;
            Oids["RIPEMD256"] = TeleTrusTObjectIdentifiers.RipeMD256;
            Oids["GOST3411"] = CryptoProObjectIdentifiers.GostR3411;
            Oids["SM3"] = GMObjectIdentifiers.sm3;
            Oids["BLAKE2B-160"] = MiscObjectIdentifiers.id_blake2b160;
            Oids["BLAKE2B-256"] = MiscObjectIdentifiers.id_blake2b256;
            Oids["BLAKE2B-384"] = MiscObjectIdentifiers.id_blake2b384;
            Oids["BLAKE2B-512"] = MiscObjectIdentifiers.id_blake2b512;
            Oids["BLAKE2S-128"] = MiscObjectIdentifiers.id_blake2s128;
            Oids["BLAKE2S-160"] = MiscObjectIdentifiers.id_blake2s160;
            Oids["BLAKE2S-224"] = MiscObjectIdentifiers.id_blake2s224;
            Oids["BLAKE2S-256"] = MiscObjectIdentifiers.id_blake2s256;
            Oids["BLAKE3-256"] = MiscObjectIdentifiers.blake3_256;
            Oids["GOST3411-2012-256"] = RosstandartObjectIdentifiers.id_tc26_gost_3411_12_256;
            Oids["GOST3411-2012-512"] = RosstandartObjectIdentifiers.id_tc26_gost_3411_12_512;
            Oids["DSTU7564-256"] = UAObjectIdentifiers.dstu7564digest_256;
            Oids["DSTU7564-384"] = UAObjectIdentifiers.dstu7564digest_384;
            Oids["DSTU7564-512"] = UAObjectIdentifiers.dstu7564digest_512;

#if DEBUG
            foreach (var key in AlgorithmMap.Keys)
            {
                if (DerObjectIdentifier.TryFromID(key, out var ignore))
                    throw new Exception("OID mapping belongs in AlgorithmOidMap: " + key);
            }

            var mechanisms = new HashSet<string>(AlgorithmMap.Values);
            mechanisms.UnionWith(AlgorithmOidMap.Values);

            foreach (var mechanism in mechanisms)
            {
                if (AlgorithmMap.TryGetValue(mechanism, out var check))
                {
                    if (mechanism != check)
                        throw new Exception("Mechanism mapping MUST be to self: " + mechanism);
                }
                else
                {
                    if (!mechanism.Equals(mechanism.ToUpperInvariant()))
                        throw new Exception("Unmapped mechanism MUST be uppercase: " + mechanism);
                }
            }
#endif
        }

        // TODO[api] Change parameter name to 'oid'
        public static byte[] CalculateDigest(DerObjectIdentifier id, byte[] input)
        {
            return CalculateDigest(id.Id, input);
        }

        public static byte[] CalculateDigest(string algorithm, byte[] input)
        {
            IDigest digest = GetDigest(algorithm);
            return DoFinal(digest, input);
        }

        public static byte[] CalculateDigest(string algorithm, byte[] buf, int off, int len)
        {
            IDigest digest = GetDigest(algorithm);
            return DoFinal(digest, buf, off, len);
        }

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static byte[] CalculateDigest(DerObjectIdentifier oid, ReadOnlySpan<byte> buffer) =>
            CalculateDigest(oid.GetID(), buffer);

        public static byte[] CalculateDigest(string algorithm, ReadOnlySpan<byte> buffer)
        {
            IDigest digest = GetDigest(algorithm);
            return DoFinal(digest, buffer);
        }
#endif

        public static byte[] DoFinal(IDigest digest)
        {
            byte[] b = new byte[digest.GetDigestSize()];
            digest.DoFinal(b, 0);
            return b;
        }

        public static byte[] DoFinal(IDigest digest, byte[] input)
        {
            digest.BlockUpdate(input, 0, input.Length);
            return DoFinal(digest);
        }

        public static byte[] DoFinal(IDigest digest, byte[] buf, int off, int len)
        {
            digest.BlockUpdate(buf, off, len);
            return DoFinal(digest);
        }

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static byte[] DoFinal(IDigest digest, ReadOnlySpan<byte> buffer)
        {
            digest.BlockUpdate(buffer);
            return DoFinal(digest);
        }
#endif

        public static string GetAlgorithmName(DerObjectIdentifier oid)
        {
            return CollectionUtilities.GetValueOrNull(AlgorithmOidMap, oid);
        }

        // TODO[api] Change parameter name to 'oid'
        public static IDigest GetDigest(DerObjectIdentifier id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (AlgorithmOidMap.TryGetValue(id, out var mechanism))
            {
                var digest = GetDigestForMechanism(mechanism);
                if (digest != null)
                    return digest;
            }

            throw new SecurityUtilityException("Digest OID not recognised.");
        }

        public static IDigest GetDigest(string algorithm)
        {
            if (algorithm == null)
                throw new ArgumentNullException(nameof(algorithm));

            string mechanism = GetMechanism(algorithm) ?? algorithm.ToUpperInvariant();

            var digest = GetDigestForMechanism(mechanism);
            if (digest != null)
                return digest;

            throw new SecurityUtilityException("Digest " + algorithm + " not recognised.");
        }

        private static IDigest GetDigestForMechanism(string mechanism)
        {
            if (!Enums.TryGetEnumValue<DigestAlgorithm>(mechanism, out var digestAlgorithm))
                return null;

            switch (digestAlgorithm)
            {
            case DigestAlgorithm.BLAKE2B_160: return new Blake2bDigest(160);
            case DigestAlgorithm.BLAKE2B_256: return new Blake2bDigest(256);
            case DigestAlgorithm.BLAKE2B_384: return new Blake2bDigest(384);
            case DigestAlgorithm.BLAKE2B_512: return new Blake2bDigest(512);
            case DigestAlgorithm.BLAKE2S_128: return new Blake2sDigest(128);
            case DigestAlgorithm.BLAKE2S_160: return new Blake2sDigest(160);
            case DigestAlgorithm.BLAKE2S_224: return new Blake2sDigest(224);
            case DigestAlgorithm.BLAKE2S_256: return new Blake2sDigest(256);
            case DigestAlgorithm.BLAKE3_256: return new Blake3Digest(256);
            case DigestAlgorithm.DSTU7564_256: return new Dstu7564Digest(256);
            case DigestAlgorithm.DSTU7564_384: return new Dstu7564Digest(384);
            case DigestAlgorithm.DSTU7564_512: return new Dstu7564Digest(512);
            case DigestAlgorithm.GOST3411: return new Gost3411Digest();
            case DigestAlgorithm.GOST3411_2012_256: return new Gost3411_2012_256Digest();
            case DigestAlgorithm.GOST3411_2012_512: return new Gost3411_2012_512Digest();
            case DigestAlgorithm.KECCAK_224: return new KeccakDigest(224);
            case DigestAlgorithm.KECCAK_256: return new KeccakDigest(256);
            case DigestAlgorithm.KECCAK_288: return new KeccakDigest(288);
            case DigestAlgorithm.KECCAK_384: return new KeccakDigest(384);
            case DigestAlgorithm.KECCAK_512: return new KeccakDigest(512);
            case DigestAlgorithm.MD2: return new MD2Digest();
            case DigestAlgorithm.MD4: return new MD4Digest();
            case DigestAlgorithm.MD5: return new MD5Digest();
            case DigestAlgorithm.NONE: return new NullDigest();
            case DigestAlgorithm.RIPEMD128: return new RipeMD128Digest();
            case DigestAlgorithm.RIPEMD160: return new RipeMD160Digest();
            case DigestAlgorithm.RIPEMD256: return new RipeMD256Digest();
            case DigestAlgorithm.RIPEMD320: return new RipeMD320Digest();
            case DigestAlgorithm.SHA_1: return new Sha1Digest();
            case DigestAlgorithm.SHA_224: return new Sha224Digest();
            case DigestAlgorithm.SHA_256: return new Sha256Digest();
            case DigestAlgorithm.SHA_384: return new Sha384Digest();
            case DigestAlgorithm.SHA_512: return new Sha512Digest();
            case DigestAlgorithm.SHA_512_224: return new Sha512tDigest(224);
            case DigestAlgorithm.SHA_512_256: return new Sha512tDigest(256);
            case DigestAlgorithm.SHA3_224: return new Sha3Digest(224);
            case DigestAlgorithm.SHA3_256: return new Sha3Digest(256);
            case DigestAlgorithm.SHA3_384: return new Sha3Digest(384);
            case DigestAlgorithm.SHA3_512: return new Sha3Digest(512);
            case DigestAlgorithm.SHAKE128_256: return new ShakeDigest(128);
            case DigestAlgorithm.SHAKE256_512: return new ShakeDigest(256);
            case DigestAlgorithm.SM3: return new SM3Digest();
            case DigestAlgorithm.TIGER: return new TigerDigest();
            case DigestAlgorithm.WHIRLPOOL: return new WhirlpoolDigest();
            default:
                throw new NotImplementedException();
            }
        }

        private static string GetMechanism(string algorithm)
        {
            if (AlgorithmMap.TryGetValue(algorithm, out var mechanism1))
                return mechanism1;

            if (DerObjectIdentifier.TryFromID(algorithm, out var oid))
            {
                if (AlgorithmOidMap.TryGetValue(oid, out var mechanism2))
                    return mechanism2;
            }

            return null;
        }

        /// <summary>
        /// Returns an ObjectIdentifier for a given digest mechanism.
        /// </summary>
        /// <param name="mechanism">A string representation of the digest meanism.</param>
        /// <returns>A DerObjectIdentifier, null if the Oid is not available.</returns>
        public static DerObjectIdentifier GetObjectIdentifier(string mechanism)
        {
            if (mechanism == null)
                throw new ArgumentNullException(nameof(mechanism));

            mechanism = GetMechanism(mechanism) ?? mechanism;

            return CollectionUtilities.GetValueOrNull(Oids, mechanism);
        }
    }
}
