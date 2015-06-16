using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Pulse.Models;
using Pulse.Services;

namespace Pulse.Dapper
{
    public class MapEventRepository : IMapEventRepository
    {
        private readonly IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["TMDATAConnection"].ConnectionString);

        private string QueryDateOffset = String.Format("DECLARE @dateNow DATETIME = DATEADD(hour,-{0},GETDATE());",
                                                       (24 - SettingsService.OffsetInHours).ToString());


        public IList<TradeMeStandaloneEvent> GetSingleMapEvents()
        {
            var query = string.Format("{0}{1}{2}{3}",
                                      QueryDateOffset,
                                      "SELECT categoryId as CategoryId, startdate as OccuredOn, s.suburbname as Suburb, r.RegionName as Region from trademe.dbo.auction a (NOLOCK) ",
                                      "INNER join trademe.dbo.suburb s (NOLOCK) on s.SuburbId = a.suburbid INNER join trademe.dbo.Region r (NOLOCK) on r.RegionId = s.Regionid ",
                                      "WHERE [startdate]>=DATEADD(minute,-10,@dateNow) AND [startdate]<@dateNow ORDER BY startdate asc;");
            return
                db.Query<TradeMeStandaloneEvent>(
                    query)
                    .ToList();
        }


        public IList<TradeMeInteractionEvent> GetInteractionMapEvents()
        {
            var query = String.Format("{0}{1}{2}{3}{4}{5}{6}",
                                      QueryDateOffset,
                                      "SELECT a.categoryId as CategoryId, vws.[sold_date] as OccuredOn ,r1.RegionName as StartRegion,s1.suburbname as StartSuburb,r2.RegionName as EndRegion,s2.suburbname as EndSuburb FROM [trademe].[dbo].[auction_sold] vws (NOLOCK) ",
                                      "INNER join [dbo].[Member] m (NOLOCK) on  vws.seller_id = m.MemberId INNER join [dbo].[Member] mm (NOLOCK) on  vws.buyer_id = mm.MemberId ",
                                      "INNER join trademe.dbo.suburb s1 (NOLOCK) on s1.SuburbId = m.suburbid INNER join trademe.dbo.Region r1 (NOLOCK) on r1.RegionId = s1.Regionid ",
                                      "INNER join trademe.dbo.suburb s2 (NOLOCK) on s2.SuburbId = mm.suburbid INNER join trademe.dbo.Region r2 (NOLOCK) on r2.RegionId = s2.Regionid ",
                                      "INNER join trademe.dbo.auction a (NOLOCK) on a.AuctionId = vws.reference_id ",
                                      "where [sold_date]>=DATEADD(minute,-10,@dateNow) AND [sold_date]<@dateNow ORDER BY sold_date asc");
            return
                db.Query<TradeMeInteractionEvent>(
                    query)
                    .ToList();
        }
        public IList<TradeMeInteractionEvent> GetComments()
        {
            var query =QueryDateOffset +  @"
SELECT a.categoryId as CategoryId, vws.[date_entered] as OccuredOn ,r1.RegionName as StartRegion,s1.suburbname as StartSuburb,r2.RegionName as EndRegion,s2.suburbname as EndSuburb  FROM [trademe].[dbo].[listing_comment] vws (NOLOCK) 
                                            INNER join trademe.dbo.auction a (NOLOCK) on a.AuctionId = vws.listing_id
									 INNER join [dbo].[Member] auctionMember (NOLOCK) on  vws.member_id = auctionMember.MemberId 
									 INNER join [dbo].[Member] commentMember (NOLOCK) on  a.MemberId = commentMember.MemberId 
                                     INNER join trademe.dbo.suburb s1 (NOLOCK) on s1.SuburbId = commentMember.suburbid 									 INNER join trademe.dbo.Region r1 (NOLOCK) on r1.RegionId = s1.Regionid 
                                      INNER join trademe.dbo.suburb s2 (NOLOCK) on s2.SuburbId = auctionMember.suburbid 
									  INNER join trademe.dbo.Region r2 (NOLOCK) on r2.RegionId = s2.Regionid 
								  and vws.response is null
  where [date_entered]>=DATEADD(minute,-10,@dateNow) AND [date_entered]<@dateNow 
union
SELECT a.categoryId as CategoryId, vws.response_date as OccuredOn ,r1.RegionName as StartRegion,s1.suburbname as StartSuburb,r2.RegionName as EndRegion,s2.suburbname as EndSuburb  FROM [trademe].[dbo].[listing_comment] vws (NOLOCK) 
                                            INNER join trademe.dbo.auction a (NOLOCK) on a.AuctionId = vws.listing_id
									 INNER join [dbo].[Member] auctionMember (NOLOCK) on  vws.member_id = auctionMember.MemberId 
									 INNER join [dbo].[Member] commentMember (NOLOCK) on  a.MemberId = commentMember.MemberId 
                                     INNER join trademe.dbo.suburb s1 (NOLOCK) on s1.SuburbId = auctionMember.suburbid 
									 INNER join trademe.dbo.Region r1 (NOLOCK) on r1.RegionId = s1.Regionid 
                                      INNER join trademe.dbo.suburb s2 (NOLOCK) on s2.SuburbId = commentMember.suburbid 
									  INNER join trademe.dbo.Region r2 (NOLOCK) on r2.RegionId = s2.Regionid 
									
                              
  where response_date>=DATEADD(minute,-10,@dateNow) AND response_date<@dateNow  ORDER BY OccuredOn asc";
            return
                db.Query<TradeMeInteractionEvent>(
                    query)
                    .ToList();
        }
        
           

        public int GetSoldToday()
        {
            var query = String.Format("{0}{1}",
                                      QueryDateOffset,
                                      "SELECT COUNT (*) FROM auction_sold (NOLOCK) WHERE sold_date>CONVERT(date, @dateNow) AND sold_date<@dateNow;");
            return db.Query<int>(query).First();
        }

        public int GetNewToday()
        {
            var query = String.Format("{0}{1}",
                                     QueryDateOffset,
                                     "SELECT COUNT (*) FROM auction (NOLOCK) WHERE StartDate>CONVERT(date, @dateNow) AND StartDate<@dateNow;");
            return db.Query<int>(query).First();
        }
    }
}