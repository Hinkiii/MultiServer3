using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.RMC;
using QuazalServer.QNetZ.Connection;
using QuazalServer.QNetZ;
using QuazalServer.RDVServices.DDL.Models.UserStorageService;
using Org.BouncyCastle.Asn1.Cms;
using QuazalServer.QNetZ.DDL;

namespace QuazalServer.RDVServices.GameServices.PS3UbisoftServices
{
    public class RMCPResponseSendTags : RMCPResponse
    {
        public override string ToString()
        {
            return "[RMCPResponseSendTags]";
        }

        public override string PayloadToString()
        {
            return "";
        }

        public override byte[] ToBuffer()
        {
            // Return empty payload
            return Array.Empty<byte>();
        }
    }
    public class TrackingInformation
    {
        public uint ipn { get; set; }
        public string userID { get; set; }
        public string machineID { get; set; }
        public string visitorID { get; set; }
        public string utsVersion { get; set; }
    }

    public class SendTagsResponse
    {
        public uint retval { get; set; }
    }
    public class UserInfoResult
    {
        public TrackingInformation TrackingInformation { get; set; }
        public uint TrackingID { get; set; }
    }

    public class TagResult
    {
        public uint TrackingId { get; set; }
        public string Tag { get; set; }
        public string Attributes { get; set; }
        public uint DeltaTime { get; set; }
    }

    public class ConfigurationResponse
    {
        public List<string> Tags { get; set; }
    }

    [RMCService((ushort)RMCProtocolId.Tracking3)]
    public class TrackingService3 : RMCServiceBase
    {
        // 0F 02 00 00 24 01 3B 00 00 00 04 80 00 00 23 00 00 00 0D 00 4C 49 4E 4B 41 50 50 5F 56 49 45 57 00 0C 00 55 50 4C 41 59 5F 53 54 41 52 54 00 0B 00 55 50 4C 41 59 5F 53 54 4F 50 00 0B 00 47 41 4D 45 5F 53 54 41 52 54 00 0A 00 47 41 4D 45 5F 53 54 4F 50 00 10 00 46 50 53 43 4C 49 45 4E 54 5F 53 54 41 52 54 00 0F 00 46 50 53 43 4C 49 45 4E 54 5F 53 54 4F 50 00 0C 00 4C 45 56 45 4C 5F 53 54 41 52 54 00 0B 00 4C 45 56 45 4C 5F 53 54 4F 50 00 10 00 4F 42 4A 45 43 54 49 56 45 5F 53 54 41 52 54 00 0F 00 4F 42 4A 45 43 54 49 56 45 5F 53 54 4F 50 00 0D 00 55 50 4C 41 59 5F 42 52 4F 57 53 45 00 0E 00 47 41 4D 45 5F 43 4F 4D 50 4C 45 54 45 00 0B 00 4C 4F 42 42 59 5F 54 49 4D 45 00 0C 00 4D 41 4E 55 41 4C 5F 54 49 4D 45 00 12 00 4D 41 54 43 48 4D 41 4B 49 4E 47 5F 53 54 41 54 53 00 09 00 4D 4D 5F 41 42 4F 52 54 00 08 00 4D 4D 5F 53 55 43 43 00 11 00 4F 50 54 49 4F 4E 41 4C 5F 43 4F 4E 54 45 4E 54 00 0B 00 53 4E 5F 4D 45 53 53 41 47 45 00 0D 00 41 57 41 52 44 5F 55 4E 4C 4F 43 4B 00 0D 00 50 4C 41 59 45 52 5F 44 45 41 54 48 00 0A 00 47 41 4D 45 5F 53 41 56 45 00 0E 00 49 4E 53 54 41 4C 4C 5F 53 54 41 52 54 00 0D 00 49 4E 53 54 41 4C 4C 5F 53 54 4F 50 00 0B 00 4D 45 4E 55 5F 45 4E 54 45 52 00 0A 00 4D 45 4E 55 5F 45 58 49 54 00 12 00 4D 45 4E 55 5F 4F 50 54 49 4F 4E 43 48 41 4E 47 45 00 07 00 4D 4D 5F 52 45 53 00 0C 00 50 4C 41 59 45 52 5F 4B 49 4C 4C 00 0D 00 50 4C 41 59 45 52 5F 53 41 56 45 44 00 10 00 55 4E 49 4E 53 54 41 4C 4C 5F 53 54 41 52 54 00 0F 00 55 4E 49 4E 53 54 41 4C 4C 5F 53 54 4F 50 00 0C 00 56 49 44 45 4F 5F 53 54 41 52 54 00 0B 00 56 49 44 45 4F 5F 53 54 4F 50 

        [RMCMethod(1)]
        public void SendTag(uint tracking_id, string tag, string attributes, uint delta_time)
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(2)]
        public void SendTagAndUpdateUserInfo(uint tracking_id, string tag, string attributes, uint delta_time, string user_id)
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(3)]
        public RMCResult SendUserInfo(TrackingInformation trackinginformation, uint delta_time)
        {
            trackinginformation.visitorID = "444";
            trackinginformation.utsVersion = "1.0.6";


            var result = new UserInfoResult
            {
                TrackingInformation = trackinginformation,
                TrackingID = 150
            };
            return Result(result);
        }

        [RMCMethod(4)]
        public RMCResult GetConfiguration()
        {
            var response = new ConfigurationResponse
            {
                Tags = new List<string>
        {
            "FIGHT_STOP",
            "UPLAY_MENU",
            "UPLAY_ACCOUNT",
            "AWARD_UNLOCK"
        }
            };

            return Result(response);
        }

        [RMCMethod(5)]
        public RMCResult SendTags(List<TrackingTag> trackingtags)
        {
            return Error(65537);
        }
    }
}
