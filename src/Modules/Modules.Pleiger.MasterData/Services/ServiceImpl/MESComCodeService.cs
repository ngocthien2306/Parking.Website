
using InfrastructureCore;
using InfrastructureCore.Models.Identity;
using System;
using System.Collections.Generic;
using InfrastructureCore.DAL;
using System.Linq;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.MasterData.Services.IService;

namespace Modules.Pleiger.MasterData.Services.ServiceImp
{
    public class MESComCodeService : IMESComCodeService
    {
        private const string SP_MES_COMMON_CODE = "SP_MES_COMMON_CODE";

        #region "MES_ComCode Master"
        public List<MES_ComCodeMst> GetListComCodeMST()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ComCodeMst>(SP_MES_COMMON_CODE,
                    new string[] { "@DIV" },
                    new object[] { "SelectMaster" }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.NO = no++;
                });

                return result;
            }
        }
        public Result DeleteGroupComCodeMST(MES_ComCodeMst item, SYLoggedUser inf)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery(SP_MES_COMMON_CODE,
                        new string[] { "@DIV", "@GROUP_CD" },
                        new object[] { "DeleteDataMaster", item.GROUP_CD });
                    if (result > 0)
                    {
                        return new Result
                        {

                            Success = true,
                            Message = "Delete success!"
                        };
                    }
                    else
                    {
                        return new Result
                        {
                            Success = false,
                            Message = "Delete not success!",
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new Result
                    {
                        Success = false,
                        Message = "Delete not success! + Exception: " + ex.ToString(),
                    };
                }

            }
        }
        private int CheckGroupComCodeMSTDuplicate(string groupCd)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ComCodeMst>(SP_MES_COMMON_CODE,
                    new string[] { "@DIV", "@GROUP_CD" },
                    new object[] { "SelectMasterDuplicate", groupCd }).Count();
                return result;
            }
        }
        private int CheckComCodeDTLDuplicate(string groupCd, string baseCd)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ComCodeMst>(SP_MES_COMMON_CODE,
                    new string[] { "@DIV", "@GROUP_CD", "@BASE_CODE" },
                    new object[] { "SelectDetailsDuplicate", groupCd, baseCd }).Count();
                return result;
            }
        }
        public Result SaveDataComCodeMST(List<MES_ComCodeMst> dataMST, List<MES_ComCodeDtls> dataDTL, List<MES_ComCodeDtls> dataDTLDelete, string groupCdSelected, SYLoggedUser info)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                Result resultAll = new Result();
                var result = -1;
                var resultMst = 0;
                var resultDtl = 0;
                var resultDl = 0;
                try
                {
                    // save master
                    if (dataMST.Count > 0)
                    {
                        foreach (var item in dataMST)
                        {
                            // insert new
                            if (item.__created__ || (item.__created__ && item.__modified__))
                            {
                                // Check Code master duplicate or not
                                int countMst = CheckGroupComCodeMSTDuplicate(item.GROUP_CD);
                                if (countMst >= 1)
                                {
                                    return resultAll = new Result
                                    {

                                        Success = true,
                                        Message = "Group code master is existed!"
                                    };
                                }
                                else
                                {
                                    result = conn.ExecuteNonQuery(SP_MES_COMMON_CODE,
                                    new string[] { "@DIV", "@GROUP_CD", "@GROUP_NM1", "@GROUP_NM2", "@DESCRIPTION", "@CREATED_BY" },
                                    new object[] { "InsertDataMaster", item.GROUP_CD, item.GROUP_NM1, item.GROUP_NM2, item.DESCRIPTION, info.UserID });
                                    resultMst += result;
                                }
                            }
                            else if (item.__modified__)
                            {
                                // Check Code master duplicate or not
                                int countMst = CheckGroupComCodeMSTDuplicate(item.GROUP_CD);
                                if (countMst >= 1)
                                {
                                    result = conn.ExecuteNonQuery(SP_MES_COMMON_CODE,
                                    new string[] { "@DIV", "@GROUP_CD", "@GROUP_NM1", "@GROUP_NM2", "@DESCRIPTION", "@UPDATED_BY" },
                                    new object[] { "UpdateDataMaster", item.GROUP_CD, item.GROUP_NM1, item.GROUP_NM2, item.DESCRIPTION, info.UserID });
                                    resultMst += result;
                                }
                                else
                                {
                                    return resultAll = new Result
                                    {
                                        Success = true,
                                        Message = "Group code master is existed!"
                                    };
                                }
                            }
                        }
                    }

                    // save details
                    if (dataDTL.Count > 0)
                    {

                        foreach (var itemDetail in dataDTL)
                        {
                            // insert new
                            if (itemDetail.__created__ || (itemDetail.__created__ && itemDetail.__modified__))
                            {
                                // Check Code  duplicate or not
                                int countDtl = CheckComCodeDTLDuplicate(itemDetail.GROUP_CD, itemDetail.BASE_CODE);
                                if (countDtl >= 1)
                                {
                                    return resultAll = new Result
                                    {
                                        Success = true,
                                        Message = "Base code is existed!"
                                    };
                                }
                                else
                                {
                                    result = conn.ExecuteNonQuery(SP_MES_COMMON_CODE,
                                    new string[] { "@DIV", "@GROUP_CD", "@BASE_CODE", "@BASE_NAME1", "@BASE_NAME2", "@BASE_NAME3",
                                                    "@BASE_NAME4", "@BASE_NAME5", "@SORT", "@CREATED_BY" },
                                    new object[] { "InsertDataDetails", groupCdSelected, itemDetail.BASE_CODE, itemDetail.BASE_NAME1, itemDetail.BASE_NAME2,
                                        itemDetail.BASE_NAME3, itemDetail.BASE_NAME4, itemDetail.BASE_NAME5, itemDetail.SORT, info.UserID });

                                    resultDtl += result;
                                }
                            }
                            else if (itemDetail.__modified__)
                            {
                                // Check Code  duplicate or not
                                int countDtl = CheckComCodeDTLDuplicate(itemDetail.GROUP_CD, itemDetail.BASE_CODE);
                                if (countDtl >= 1)
                                {
                                    result = conn.ExecuteNonQuery(SP_MES_COMMON_CODE,
                                        new string[] { "@DIV", "@GROUP_CD", "@BASE_CODE", "@BASE_NAME1", "@BASE_NAME2", "@BASE_NAME3",
                                                    "@BASE_NAME4", "@BASE_NAME5", "@SORT", "@USE_YN", "@CREATED_BY" },
                                        new object[] { "UpdateDataDetails", itemDetail.GROUP_CD, itemDetail.BASE_CODE, itemDetail.BASE_NAME1, itemDetail.BASE_NAME2,
                                                    itemDetail.BASE_NAME3, itemDetail.BASE_NAME4, itemDetail.BASE_NAME5, itemDetail.SORT, itemDetail.USE_YN, info.UserID });

                                    resultDtl += result;
                                }
                                else
                                {
                                    return resultAll = new Result
                                    {
                                        Success = true,
                                        Message = "Base code is existed!"
                                    };
                                }
                            }
                        }
                    }

                    // delete details
                    if (dataDTLDelete.Count() > 0)
                    {
                        foreach (var itemDetailDelete in dataDTLDelete)
                        {
                            //delete
                            if (itemDetailDelete.__deleted__)
                            {
                                result = conn.ExecuteNonQuery(SP_MES_COMMON_CODE,
                                        new string[] { "@DIV", "@GROUP_CD", "@BASE_CODE" },
                                        new object[] { "DeleteDataDetails", itemDetailDelete.GROUP_CD, itemDetailDelete.BASE_CODE });

                                resultDl += result;
                            }
                        }
                    }
                    if ((resultMst + resultDtl + resultDl) > 0)
                    {
                        return resultAll = new Result
                        {

                            Success = true,
                            Message = "Save changed data success!"
                        };
                    }
                    else
                    {
                        return resultAll = new Result
                        {
                            Success = false,
                            Message = "Save changed data not success!",
                        };
                    }

                }
                catch (Exception ex)
                {
                    return new Result
                    {
                        Success = false,
                        Message = "Save changed data not success! + Exception: " + ex.ToString(),
                    };
                }
                return resultAll;
            }
        }

        #endregion

        #region "MES_ComCode Master"

        public List<MES_ComCodeDtls> GetListComCodeDTL(string groupCD)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ComCodeDtls>(SP_MES_COMMON_CODE,
                    new string[] { "@DIV", "@GROUP_CD" },
                    new object[] { "SelectDetails", groupCD }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.NO = no++;
                });

                return result;
            }
        }
        public List<MES_ComCodeDtls> GetListComCodeCategoryMaterial(string groupCD, string baseCD1, string baseCD2, string baseCD3, string baseCD4)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ComCodeDtls>(SP_MES_COMMON_CODE,
                    new string[] { "@DIV", "@GROUP_CD","@BASE_CD1", "@BASE_CD2", "@BASE_CD3", "@BASE_CD4" },
                    new object[] { "GetListComCodeCategoryMaterial", groupCD, baseCD1, baseCD2, baseCD3, baseCD4 }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.NO = no++;
                });

                return result;
            }
        }
        
        public List<MES_ComCodeDtls> GetListComCodeDTLAll(string groupCD)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ComCodeDtls>(SP_MES_COMMON_CODE,
                    new string[] { "@DIV", "@GROUP_CD" },
                    new object[] { "SelectAllBycategory", groupCD }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.NO = no++;
                });

                return result;
            }
        }
        public List<MES_ComCodeDtls> GetListComCodeDTLProduct()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ComCodeDtls>(SP_MES_COMMON_CODE,
                    new string[] { "@DIV"},
                    new object[] { "GetListComCodeDTLProduct" }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.NO = no++;
                });

                return result;
            }
        }
        
        public Result SaveDataComCodeDTL(MES_ComCodeDtls item, SYLoggedUser info)
        {
            throw new NotImplementedException();
        }

        public Result DeleteGroupComCodeDTL(MES_ComCodeDtls item, SYLoggedUser info)
        {
            throw new NotImplementedException();
        }
        #endregion

       

    }
}
