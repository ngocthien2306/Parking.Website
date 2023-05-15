using Microsoft.Extensions.Configuration;
using Modules.Parking.Hubs;
using Modules.Pleiger.CommonModels.Parking;
using System;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;

namespace Modules.Parking.TableDependencies
{
    public class TrackDependency
    {
        SqlTableDependency<tblTrack> tableDependency;

        TrackHub monitoring;
        public IConfiguration configuration;

        public TrackDependency(TrackHub monitoring, IConfiguration configuration)
        {
            this.monitoring = monitoring;
            this.configuration = configuration;
        }

        public void SubcribeTableDependency()
        {
            string connectionString = configuration.GetConnectionString("DbConnection1:ConnectionString");
            tableDependency = new SqlTableDependency<tblTrack>(connectionString);
            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();
        }

        private void TableDependency_OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"{nameof(tblTrack)} SQLTableDependency error: {e.Error.Message}");
        }

        private async void TableDependency_OnChanged(object sender, RecordChangedEventArgs<tblTrack> e)
        {
            if (e.ChangeType == TableDependency.SqlClient.Base.Enums.ChangeType.Insert || e.ChangeType == TableDependency.SqlClient.Base.Enums.ChangeType.Update)
            {
                await monitoring.SendTracks();
            }
        }
    }
}
