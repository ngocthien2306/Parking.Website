using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Common.Models;
using Modules.Parking.Repositories.IRepo;
using Modules.Pleiger.CommonModels.Parking;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Modules.Parking.Repositories.Repo
{
    class ParkingRepository : IParkingRepository
    {
        private readonly static string PARKING_MANAGEMENT = "PARKING_MANAGEMENT";

        public List<ParkingMaster> GetListParkingMaster(string id)
        {
            List<ParkingMaster> parkings = new List<ParkingMaster>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[2];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@Id";
                    object[] arrValue = new object[2];
                    arrValue[0] = "GetParkingMaster";
                    arrValue[1] = id;
                    parkings = connection.ExecuteQuery<ParkingMaster>(PARKING_MANAGEMENT, arrParam, arrValue).ToList();
                    return parkings;
                }
            }
            catch
            {
                return parkings;
            }
        }
        public Result DeletParking(int sideId)
        {
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var resultDelete = "false";
                            string[] arrParam = new string[2];
                            arrParam[0] = "@Method";
                            arrParam[1] = "@AdNo";
                            object[] arrValue = new object[2];
                            arrValue[0] = "DeleteParking";
                            arrValue[1] = sideId;
                            resultDelete = connection.ExecuteScalar<string>(PARKING_MANAGEMENT, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                            transaction.Commit();
                            if (resultDelete == "1")
                            {
                                return new Result { Success = true, Message = MessageCode.MEAD03 };
                            }
                            else
                            {
                                transaction.Rollback();
                                return new Result { Success = false, Message = MessageCode.MEAD04 };
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            return new Result { Success = false, Message = MessageCode.MEAD04 };

                        }
                    }
                }
            }
            catch
            {
                return new Result { Success = false, Message = MessageCode.MEAD04 };
            }
        }
        public Result SaveParkingMaster(ParkingMaster parkingMaster)
        {
            try
            {
                using (var conneciton = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using (var transaction = conneciton.BeginTransaction())
                    {
                        try
                        {
                            var resultSave = "false";
                            string[] arrParam = new string[9];
                            arrParam[0] = "@Method"; arrParam[1] = "@NameParking"; arrParam[2] = "@Capacity";
                            arrParam[3] = "@Location"; arrParam[4] = "@ZipCode"; arrParam[5] = "@Address1";
                            arrParam[6] = "@Address2"; arrParam[7] = "@PhoneNumber";
                            arrParam[8] = "@OpenDate";
                            object[] arrValue = new object[9];
                            arrValue[0] = "SaveParking"; arrValue[1] = parkingMaster.NameParking; arrValue[2] = parkingMaster.Capacity;
                            arrValue[3] = parkingMaster.Location;
                            arrValue[4] = parkingMaster.ZipCode;
                            arrValue[5] = parkingMaster.Address1;
                            arrValue[6] = parkingMaster.Address2;
                            arrValue[7] = parkingMaster.PhoneNumber;
                            arrValue[8] = parkingMaster.OpenDate?.ToString("yyyy-MM-dd HH:mm:ss");

                            resultSave = conneciton.ExecuteScalar<string>(PARKING_MANAGEMENT, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                            transaction.Commit();
                            if (resultSave != "false")
                            {
                                return new Result { Success = true, Message = MessageCode.MEAD01, Data = resultSave };
                            }
                            else
                            {
                                transaction.Rollback();
                                return new Result { Success = false, Message = MessageCode.MEAD02 };
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            return new Result { Success = false, Message = MessageCode.MEAD02 };
                        }
                    }
                }
            }
            catch
            {
                return new Result { Success = false, Message = MessageCode.MEAD02 };
            }
        }
    }
}
