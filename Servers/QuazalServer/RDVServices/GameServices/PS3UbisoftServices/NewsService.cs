using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.RMC;
using QuazalServer.RDVServices.DDL.Models.UserStorageService;
using NetworkLibrary.GeoLocalization;

namespace QuazalServer.RDVServices.GameServices.PS3UbisoftServices
{
    /// <summary>
    /// Ubi news service
    /// </summary>
    [RMCService((ushort)RMCProtocolId.NewsService)]
    public class NewsService : RMCServiceBase
    {
        [RMCMethod(1)]
        public void GetChannels()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(2)]
        public void GetChannelsByTypes()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(3)]
        public void GetSubscribableChannels()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(4)]
        public void GetChannelsByIDs()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(5)]
        public void GetSubscribedChannels()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(6)]
        public void SubscribeChannel()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(7)]
        public void UnsubscribeChannel()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(8)]
        public RMCResult GetNewsHeaders(NewsRecipient recipient, uint offset, uint size)
        {
            var plInfo = Context?.Client.PlayerInfo;
            var random = new Random();

            var funNews = new List<string>{
                "Actumcnally",
                $"Hello { plInfo?.Name }! Welcome to Alcatraz server!",
                $"Players online: { NetworkPlayers.Players.Count-1 }",
                "Subscribe to VortexStory on YouTube!",
                "Support SoapyMan with coffee!",
            };

            var headers = funNews.Select((x, idx) => new NewsHeader
            {
                m_ID = (uint)idx + 1,
                m_publisherName = "SoapyMan",
                m_title = x,
                m_link = string.Empty,
                m_displayTime = DateTime.UtcNow,
                m_expirationTime = DateTime.UtcNow.AddDays(10),
                m_publicationTime = new DateTime(2000, 10, 12, 13, 0, 0),
                m_publisherPID = Context.Client.sPID,
                m_recipientID = Context.Client.IDsend,
                m_recipientType = 0,
            });

            return Result(headers);
        }

        [RMCMethod(9)]
        public RMCResult GetNewsMessages(IEnumerable<uint> messageIds)
        {
            return Result(new List<string>
            {
                "This text apparently doesn't work."
            });
        }

        [RMCMethod(10)]
        public RMCResult GetNumberOfNews(NewsRecipient recipient)
        {
            return Result(new { number_of_news = 0 });
        }

        [RMCMethod(11)]
        public void GetChannelByType()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(12)]
        public void GetNewsHeadersByType()
        {
            UNIMPLEMENTED();
        }

        public class ResultRange
        {
            public uint offset { get; set; }
            public uint size { get; set; }
        }
        public class GetNewsMessagesByTypeQuery
        {
            public string newsChannelType { get; set; }
            public ResultRange result_range { get; set; }
        }


        private static uint _newsIdCounter = 1;
        [RMCMethod(13)]
        public RMCResult GetNewsMessagesByType(string newschanneltype, ResultRange result_range)
        {

            var funNews = new List<string>{
                "Actumcnally",
                $"Players online: { NetworkPlayers.Players.Count-1 }",
                "Subscribe to VortexStory on YouTube!",
                "Support SoapyMan with coffee!",
            };

            string selectedTitle = newschanneltype switch
            {
                "UbiNews" => funNews[0],
                "UbiNewsMap" => funNews[1],
                "UbiNewsLoading" => funNews[2],
                _ => "Unknown news channel"
            };

            var headers = new List<NewsHeader>
            {
                new NewsHeader
                {
                    m_ID = _newsIdCounter++,
                    m_publisherName = "SoapyMan",
                    m_title = selectedTitle,
                    m_link = string.Empty,
                    m_displayTime = DateTime.UtcNow,
                    m_expirationTime = DateTime.UtcNow.AddDays(10),
                    m_publicationTime = new DateTime(2000, 10, 12, 13, 0, 0),
                    m_publisherPID = Context.Client.sPID,
                    m_recipientID = Context.Client.IDsend,
                    m_recipientType = 0,
                }
            };

            return Result(headers);
        }
    }
    }

