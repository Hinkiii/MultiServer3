using NetworkLibrary.GeoLocalization;
using QuazalServer.QNetZ;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.RDVServices.DDL.Models.UserStorageService;
using QuazalServer.RDVServices.RMC;

namespace QuazalServer.RDVServices.GameServices.PS3UbisoftServices
{
    [RMCService((ushort)RMCProtocolId.UplayStorageService)]

    public class UplayStorageService : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult SearchContents()
        {

            var range = new ResultRange
            {
                muiOffset = 0,
                muiSize = 100
            };
            var headers = new UserStorageQuery
            {
                type_id = 0,
                query_id = NetworkPlayers.GenerateUniqueUint("Hinki" + "a1nPut!"),
                result_range = range,
                m_attributes = new List<GameSessionProperty>(),
            };

            return Result(headers);
        }

        [RMCMethod(2)]
        public RMCResult SearchContentsWithTotal()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(3)]
        public RMCResult DeleteContent()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(4)]
        public RMCResult SaveMetaData()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(5)]
        public RMCResult SaveContentDB()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(6)]
        public RMCResult SaveContentAndGetUploadInfo()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(7)]
        public RMCResult UploadEnd()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(8)]
        public RMCResult GetContentDB()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(9)]
        public RMCResult GetContentURL()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(10)]
        public RMCResult GetSlotCount()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(11)]
        public RMCResult GetMetaData()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(12)]
        public RMCResult Like()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(13)]
        public RMCResult Unlike()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(14)]
        public RMCResult IsLiked()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(15)]
        public RMCResult GetFavourites()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(16)]
        public RMCResult MakeFavourite()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(17)]
        public RMCResult RemoveFromFavourites()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(18)]
        public RMCResult ReportInappropriate()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(19)]
        public RMCResult IncrementPlayCount()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(20)]
        public RMCResult UpdateCustomStat()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(21)]
        public RMCResult GetOwnContents()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(22)]
        public RMCResult GetMostPopularTags()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(23)]
        public RMCResult GetTags()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(24)]
        public RMCResult TagContent()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(25)]
        public RMCResult SearchContentsByPlayers()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
        [RMCMethod(26)]
        public RMCResult SearchContentsByPlayersWithTotal()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
    }
}
