using Microsoft.Extensions.Configuration;
using Modules.Kiosk.Monitoring.Hubs;
using Modules.Pleiger.CommonModels.Models;
using System;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;

namespace Modules.Kiosk.Monitoring.TableDependencies
{
    public class SubcribeCheckInTableDependency
    {
        SqlTableDependency<tblUserHistory> tableDependency;
        SqlTableDependency<tblTempDeploy> tableTempDependency;
        private readonly IConfiguration configuration;
        CheckInHub checkInHub;

        public SubcribeCheckInTableDependency(CheckInHub checkIn, IConfiguration configuration)
        {
            this.checkInHub = checkIn;
            this.configuration = configuration;
        }

        public void SubcribeTableDependency()
        {
            string connectionString = configuration.GetConnectionString("DbConnection1:ConnectionString");
            tableDependency = new SqlTableDependency<tblUserHistory>(connectionString);
            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();


        }

        private void TableDependency_OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"{nameof(tblUserHistory)} SQLTableDependency error: {e.Error.Message}");
        }

        private async void TableDependency_OnChanged(object sender, RecordChangedEventArgs<tblUserHistory> e)
        {
            if(e.ChangeType == TableDependency.SqlClient.Base.Enums.ChangeType.Insert)
            {
                await checkInHub.SendCheckIns();
            }
        }
    }
}
