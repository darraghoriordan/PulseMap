﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Pulse.Models;

namespace Pulse.Dapper
{
    public class MapEventRepository : IMapEventRepository
    {
        protected SqlConnection GetConnection()
        { return new SqlConnection(ConfigurationManager.ConnectionStrings["TMDATAConnection"].ConnectionString); }

        private DateTime OffSetDateTimeForDataWarehouse(DateTime input)
        {
            return input.AddDays(-1);
        }

        private bool DateTimeDifferenceIsSane(DateTime startDate, DateTime endDate)
        {
            if (endDate.Subtract(startDate).TotalMinutes >= 30){
                return false;
            }
            return true;
        }
        public IList<TradeMeStandaloneEvent> GetSingleMapEvents(DateTime startDate, DateTime endDate)
        {
            if (!DateTimeDifferenceIsSane(startDate, endDate))
            {
                return new List<TradeMeStandaloneEvent>();
            }
            startDate = OffSetDateTimeForDataWarehouse(startDate);
            endDate = OffSetDateTimeForDataWarehouse(endDate);
            using (var conn = GetConnection())
            {
                return
                    conn.Query<TradeMeStandaloneEvent>(
                                 @"SELECT categoryId as CategoryId, startdate as OccuredOn, s.suburbname as Suburb, r.RegionName as Region from trademe.dbo.auction a (NOLOCK)
                                      INNER join trademe.dbo.suburb s (NOLOCK) on s.SuburbId = a.suburbid INNER join trademe.dbo.Region r (NOLOCK) on r.RegionId = s.Regionid
                                      WHERE [startdate]>=@startDate AND [startdate]<@endDate ORDER BY startdate asc;", new { startDate, endDate })
                        .ToList();
            }
        }

        public IList<TradeMeInteractionEvent> GetInteractionMapEvents(DateTime startDate, DateTime endDate)
        {
            if (!DateTimeDifferenceIsSane(startDate, endDate))
            {
                return new List<TradeMeInteractionEvent>();
            }

            startDate = OffSetDateTimeForDataWarehouse(startDate);
            endDate = OffSetDateTimeForDataWarehouse(endDate);
            using (var conn = GetConnection())
            {
                return
                conn.Query<TradeMeInteractionEvent>(
                                                          @"SELECT a.categoryId as CategoryId, vws.[sold_date] as OccuredOn ,r1.RegionName as StartRegion,s1.suburbname as StartSuburb,r2.RegionName as EndRegion,s2.suburbname as EndSuburb FROM [trademe].[dbo].[auction_sold] vws (NOLOCK) 
                                     INNER join [dbo].[Member] m (NOLOCK) on  vws.seller_id = m.MemberId INNER join [dbo].[Member] mm (NOLOCK) on  vws.buyer_id = mm.MemberId
                                     INNER join trademe.dbo.suburb s1 (NOLOCK) on s1.SuburbId = m.suburbid INNER join trademe.dbo.Region r1 (NOLOCK) on r1.RegionId = s1.Regionid 
                                     INNER join trademe.dbo.suburb s2 (NOLOCK) on s2.SuburbId = mm.suburbid INNER join trademe.dbo.Region r2 (NOLOCK) on r2.RegionId = s2.Regionid 
                                     INNER join trademe.dbo.auction a (NOLOCK) on a.AuctionId = vws.reference_id
                                     where [sold_date]>=@startDate AND [sold_date]<@endDate ORDER BY sold_date asc", new { startDate, endDate })
                    .ToList();
            }
        }
        public IList<TradeMeInteractionEvent> GetComments(DateTime startDate, DateTime endDate)
        {
            if (!DateTimeDifferenceIsSane(startDate, endDate))
            {
                return new List<TradeMeInteractionEvent>();
            }

            startDate = OffSetDateTimeForDataWarehouse(startDate);
            endDate = OffSetDateTimeForDataWarehouse(endDate);
            var query = @"
SELECT a.categoryId as CategoryId, vws.[date_entered] as OccuredOn ,r1.RegionName as StartRegion,s1.suburbname as StartSuburb,r2.RegionName as EndRegion,s2.suburbname as EndSuburb  FROM [trademe].[dbo].[listing_comment] vws (NOLOCK) 
                                            INNER join trademe.dbo.auction a (NOLOCK) on a.AuctionId = vws.listing_id
									 INNER join [dbo].[Member] auctionMember (NOLOCK) on  vws.member_id = auctionMember.MemberId 
									 INNER join [dbo].[Member] commentMember (NOLOCK) on  a.MemberId = commentMember.MemberId 
                                     INNER join trademe.dbo.suburb s1 (NOLOCK) on s1.SuburbId = commentMember.suburbid 									 INNER join trademe.dbo.Region r1 (NOLOCK) on r1.RegionId = s1.Regionid 
                                      INNER join trademe.dbo.suburb s2 (NOLOCK) on s2.SuburbId = auctionMember.suburbid 
									  INNER join trademe.dbo.Region r2 (NOLOCK) on r2.RegionId = s2.Regionid 
								  and vws.response is null
  where [date_entered]>=@startDate AND [date_entered]<@endDate
union
SELECT a.categoryId as CategoryId, vws.response_date as OccuredOn ,r1.RegionName as StartRegion,s1.suburbname as StartSuburb,r2.RegionName as EndRegion,s2.suburbname as EndSuburb  FROM [trademe].[dbo].[listing_comment] vws (NOLOCK) 
                                            INNER join trademe.dbo.auction a (NOLOCK) on a.AuctionId = vws.listing_id
									 INNER join [dbo].[Member] auctionMember (NOLOCK) on  vws.member_id = auctionMember.MemberId 
									 INNER join [dbo].[Member] commentMember (NOLOCK) on  a.MemberId = commentMember.MemberId 
                                     INNER join trademe.dbo.suburb s1 (NOLOCK) on s1.SuburbId = auctionMember.suburbid 
									 INNER join trademe.dbo.Region r1 (NOLOCK) on r1.RegionId = s1.Regionid 
                                      INNER join trademe.dbo.suburb s2 (NOLOCK) on s2.SuburbId = commentMember.suburbid 
									  INNER join trademe.dbo.Region r2 (NOLOCK) on r2.RegionId = s2.Regionid 
									
                              
  where response_date>=@startDate AND response_date<@endDate  ORDER BY OccuredOn asc";

            using (var conn = GetConnection())
            {
                return
                conn.Query<TradeMeInteractionEvent>(
                    query, new { startDate, endDate })
                    .ToList();
            }
        }

        public int GetSoldToday(DateTime startDate, DateTime endDate)
        {
            startDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);
            startDate = OffSetDateTimeForDataWarehouse(startDate);
            endDate = OffSetDateTimeForDataWarehouse(endDate);
            var query = @"SELECT COUNT (*) FROM auction_sold (NOLOCK) WHERE sold_date>@startDate AND sold_date<@endDate;";

            using (var conn = GetConnection())
            {
                return conn.Query<int>(query, new { startDate, endDate }).First();
            }
        }

        public int GetNewToday(DateTime startDate, DateTime endDate)
        {
            startDate = new DateTime(endDate.Year,endDate.Month, endDate.Day);
            startDate = OffSetDateTimeForDataWarehouse(startDate);
            endDate = OffSetDateTimeForDataWarehouse(endDate);
            var query = @"SELECT COUNT (*) FROM auction (NOLOCK) WHERE StartDate>@startDate AND StartDate<@endDate;";
            using (var conn = GetConnection())
            {
                return conn.Query<int>(query, new { startDate, endDate }).First();
            }
        }

        public int GetDealerGmsToday(DateTime startDate, DateTime endDate)
        {
            startDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);
            startDate = OffSetDateTimeForDataWarehouse(startDate);
            endDate = OffSetDateTimeForDataWarehouse(endDate);
            var query = @"SELECT sum(startprice) from trademe.dbo.auction a with (NOLOCK)
                            WHERE [startdate]>=@startDate AND [startdate]<@endDate 
                            and memberid = 5633;";

            using (var conn = GetConnection())
            {
                return conn.Query<int>(query, new { startDate, endDate }).First();
            }
        }

        public IList<DealerGmsStatModel> GetLatestTotalDealerGms(DateTime startDate, DateTime endDate)
        {
            endDate = OffSetDateTimeForDataWarehouse(endDate);
            startDate = OffSetDateTimeForDataWarehouse(startDate);
            if (!DateTimeDifferenceIsSane(startDate, endDate))
            {
                return new List<DealerGmsStatModel>();
            }

            using (var conn = GetConnection())
            {
                var models =
                    conn.Query<DealerGmsStatModel>(
                            @"SELECT startprice as StartStat, startdate as OccuredOn from trademe.dbo.auction a with (NOLOCK)
                                    WHERE [startdate]>=@startDate AND [startdate]<@endDate 
                                    and memberid = 5633
                                    ORDER BY startdate asc;", new { startDate, endDate }).ToList();
                return models;
            }
        }
    }
}