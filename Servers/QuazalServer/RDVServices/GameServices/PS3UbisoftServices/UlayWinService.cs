using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.QNetZ;
using QuazalServer.RDVServices.DDL.Models.UserStorageService;
using System.Drawing.Drawing2D;

namespace QuazalServer.RDVServices.GameServices.PS3UbisoftServices
{
    /// <summary>
    /// Ubi achievements service
    /// </summary>
    /// 

    [RMCService((ushort)RMCProtocolId.UplayWinService)]
    public class UplayWinService : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult GetActions(int start_row_index, int maximum_rows, string sort_expression, string culture_name, string platform_Code, string game_Code)
        {
            //UNIMPLEMENTED();

            //var actions = new List<UplayAction>()
            //{
            //    new UplayAction()
            //    {
            //        m_code = "FinishPlaythroughs",
            //        m_name = "Exclusive Wallpaper",
            //        m_description = "Download the DRIVER San Francisco Wallpaper.",
            //        m_value = 10,
            //        m_gameCode = "SPA",
            //        m_platforms = new List<UplayActionPlatform>()
            //        {
            //            new UplayActionPlatform()
            //            {
            //                m_platformCode = "PS3",
            //                m_completed = true,
            //                m_specificKey = "TEST"
            //            }
            //        }
            //    },
            //    new UplayAction()
            //    {
            //        m_code = "OwnedStyles",
            //        m_name = "Exclusive Wallpaper",
            //        m_description = "Download the DRIVER San Francisco Wallpaper.",
            //        m_value = 10,
            //        m_gameCode = "SPA",
            //        m_platforms = new List<UplayActionPlatform>()
            //        {
            //            new UplayActionPlatform()
            //            {
            //                m_platformCode = "PS3",
            //                m_completed = true,
            //                m_specificKey = string.Empty
            //            }
            //        }
            //    },
            //};


            //return Result(actions);

            var actionList = new List<UplayAction>()
            {
                new UplayAction() // useless but we still adding it
                {
                    m_code = "SPAACT01",
                    m_name = "Exclusive Wallpaper",
                    m_description = "Download the DRIVER San Francisco Wallpaper.",
                    m_value = 10,
                    m_gameCode = game_Code,
                    m_platforms = new List<UplayActionPlatform>()
                    {
                        new UplayActionPlatform()
                        {
                            m_platformCode = platform_Code,
                            m_completed = true,
                            m_specificKey = ""
                        }
                    }
                },
                new UplayAction()
                {
                    m_code = "SPAACT02",
                    m_name = "Tanner's Day Off Challenge",
                    m_description = "Tear through Russian Hill in Tannerâ\u0080\u0099s iconic Dodge Challenger.",
                    m_value = 20,
                    m_gameCode = game_Code,
                    m_platforms = new List<UplayActionPlatform>()
                    {
                        new UplayActionPlatform()
                        {
                            m_platformCode = platform_Code,
                            m_completed = true,
                            m_specificKey = ""
                        }
                    }
                },
                new UplayAction()
                {
                    m_code = "SPAACT03",
                    m_name = "Dodge Charger SRT8 Police Car",
                    m_description = "Unlocks the Dodge Charger SRT8 Police Car for use in Online games.",
                    m_value = 30,
                    m_gameCode = game_Code,
                    m_platforms = new List<UplayActionPlatform>()
                    {
                        new UplayActionPlatform()
                        {
                            m_platformCode = platform_Code,
                            m_completed = true,
                            m_specificKey = ""
                        }
                    }
                },
                new UplayAction()
                {
                    m_code = "SPAACT04",
                    m_name = "San Francisco Challenges",
                    m_description = "Four Challenges that showcase different areas of San Francisco.",
                    m_value = 40,
                    m_gameCode = game_Code,
                    m_platforms = new List<UplayActionPlatform>()
                    {
                        new UplayActionPlatform()
                        {
                            m_platformCode = platform_Code,
                            m_completed = true,
                            m_specificKey = ""
                        }
                    }
                },
            };

            // return 
            return Result(actionList);

        }
        

        [RMCMethod(2)]
        public RMCResult GetActionsCompleted(int start_row_index, int maximum_rows, string sort_expression, string culture_name)
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(3)]
        public RMCResult GetActionsCount(string game_code)
        {
            UNIMPLEMENTED();

            int actions_count = 0;
            return Result(new { actions_count });
        }

        [RMCMethod(4)]
        public RMCResult GetActionsCompletedCount(string game_code)
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(5)]
        public RMCResult GetRewards(int start_row_index, int maximum_rows, string sort_expression, string culture_name, string platform_Code, string game_Code)
        {
            var rewardList = new List<UPlayReward>()
            {
                new UPlayReward() // useless but we still adding it
                {
                    m_code = "SPAREWARD01",
                    m_name = "Exclusive Wallpaper",
                    m_description = "Download the DRIVER San Francisco Wallpaper.",
                    m_rewardTypeName = "Downloadable",
                    m_gameCode = game_Code,
                    m_value = 50,
                    m_platforms = new List<UPlayRewardPlatform>()
                    {
                        new UPlayRewardPlatform()
                        {
                            m_platformCode = platform_Code,
                            m_purchased = false
                        }
                    }
                },
                new UPlayReward()
                {
                    m_code = "SPAREWARD02",
                    m_name = "Tanner's Day Off Challenge",
                    m_description = "Tear through Russian Hill in Tannerâ\u0080\u0099s iconic Dodge Challenger.",
                    m_rewardTypeName = "Downloadable",
                    m_gameCode = game_Code,
                    m_value = 20,
                    m_platforms = new List<UPlayRewardPlatform>()
                    {
                        new UPlayRewardPlatform()
                        {
                            m_platformCode = platform_Code,
                            m_purchased = false
                        }
                    }
                },
                new UPlayReward()
                {
                    m_code = "SPAREWARD03",
                    m_name = "Dodge Charger SRT8 Police Car",
                    m_description = "Unlocks the Dodge Charger SRT8 Police Car for use in Online games.",
                    m_rewardTypeName = "Downloadable",
                    m_gameCode = game_Code,
                    m_value = 30,
                    m_platforms = new List<UPlayRewardPlatform>()
                    {
                        new UPlayRewardPlatform()
                        {
                            m_platformCode = platform_Code,
                            m_purchased = false
                        }
                    }
                },
                new UPlayReward()
                {
                    m_code = "SPAREWARD04",
                    m_name = "San Francisco Challenges",
                    m_description = "Four Challenges that showcase different areas of San Francisco.",
                    m_rewardTypeName = "Downloadable",
                    m_gameCode = game_Code,
                    m_value = 40,
                    m_platforms = new List<UPlayRewardPlatform>()
                    {
                        new UPlayRewardPlatform()
                        {
                            m_platformCode = platform_Code,
                            m_purchased = false
                        }
                    }
                },
            };

            // return 
            return Result(rewardList);
        }

        [RMCMethod(6)]
        public RMCResult GetRewardsPurchased(int startRowIndex, int maximumRows, string sortExpression, string cultureName, string platformcode, string gamecode)
        {
            //UNIMPLEMENTED();

            var rewards = new List<UPlayReward>();

            // return 
            return Result(rewards);
        }

        [RMCMethod(7)]
        public RMCResult UplayWelcome(string culture, string platformcode, string gamecode)
        {

            var actions = new List<UplayAction>()
            {
                new UplayAction()
                {
                    m_code = "FinishPlaythroughs",
                    m_name = "Exclusive Wallpaper",
                    m_description = "Download the DRIVER San Francisco Wallpaper.",
                    m_value = 10,
                    m_gameCode = "SPA",
                    m_platforms = new List<UplayActionPlatform>()
                    {
                        new UplayActionPlatform()
                        {
                            m_platformCode = "PS3",
                            m_completed = true,
                            m_specificKey = "TEST"
                        }
                    }
                },
                new UplayAction()
                {
                    m_code = "OwnedStyles",
                    m_name = "Exclusive Wallpaper",
                    m_description = "Download the DRIVER San Francisco Wallpaper.",
                    m_value = 10,
                    m_gameCode = "SPA",
                    m_platforms = new List<UplayActionPlatform>()
                    {
                        new UplayActionPlatform()
                        {
                            m_platformCode = "PS3",
                            m_completed = true,
                            m_specificKey = string.Empty
                        }
                    }
                },
            };


            return Result(actions);
        }

        [RMCMethod(8)]
        public RMCResult SetActionCompleted(string actionCode, string cultureName)
        {
            UNIMPLEMENTED();
            var unlockedAction = new UplayAction()
            {
                m_code = actionCode,
                m_description = actionCode + "_description",
                m_gameCode = "UNK",
                m_name = actionCode + "_action",
                m_value = 1,
            };
            unlockedAction.m_platforms.Add(new UplayActionPlatform()
            {
                m_completed = true,
                m_platformCode = "PS3",
                m_specificKey = string.Empty
            });

            return Result(unlockedAction);
        }

        [RMCMethod(9)]
        public RMCResult SetActionsCompleted(IEnumerable<string> actionCodeList, string cultureName)
        {
            var actionList = new List<UplayAction>();
            return Result(actionList);
        }

        [RMCMethod(10)]
        public RMCResult GetUserToken()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(11)]
        public RMCResult GetVirtualCurrencyUserBalance()
        {
            int numOfTokens = 0;

            if (Context != null && Context.Client.PlayerInfo != null && !string.IsNullOrEmpty(Context.Client.PlayerInfo.Name))
            {
                string tokenProfileDataPath = QuazalServerConfiguration.QuazalStaticFolder + $"/Database/Uplay/account_data/currency/{Context.Client.PlayerInfo.Name}.txt";

                if (File.Exists(tokenProfileDataPath) && int.TryParse(File.ReadAllText(tokenProfileDataPath), out int localNumOfTokens))
                    numOfTokens = localNumOfTokens;
            }

            return Result(new { numOfTokens });
        }

        [RMCMethod(12)]
        public RMCResult GetSectionsByKey(string culture_name, string section_key)
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(13)]
        public RMCResult BuyReward(string reward_code)
        {
            UNIMPLEMENTED();
            return Error(0);
        }
    }
}
