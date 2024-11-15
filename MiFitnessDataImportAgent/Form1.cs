using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace MiFitnessDataImportAgent
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btImportSportData_Click(object sender, EventArgs e)
        {
            lerSportCSV();
        }

        private void lerSportCSV()
        {
            AcessoDados _bd = new AcessoDados();
            string caminhoBase = @"C:\Users\dssca\Downloads\20241116__MiFitness_c3_data_copy\";
            string arquivoSport = @"20241116__MiFitness_hlth_center_sport_record.csv";
            string arquivoFitness = @"20241116__MiFitness_hlth_center_fitness_data.csv";

            using (TextFieldParser parser = new TextFieldParser(caminhoBase + arquivoSport))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                int lineNumber = 1;

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    if (lineNumber > 1)
                    {
                        string uid = fields[0];
                        string sid = fields[1];
                        string key = fields[2];
                        string time = fields[3];
                        string category = fields[4];
                        string value = fields[5];
                        string updatetime = fields[6];

                        string sqlConsultaSport = string.Format("select 1 from sportrecord " +
                            " where [key] = '{0}' and time = '{1}' and value = '{2}' ",
                            key, time, value);

                        DataTable dt = _bd.listar(sqlConsultaSport).Tables[0];
                        if (dt.Rows.Count <= 0)
                        {
                            string sqlInsertSport = @"insert into sportrecord (Uid,Sid,[Key],Time,Category,Value,UpdateTime) ";
                            sqlInsertSport += string.Format("values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                                uid, sid, key, time, category, value, updatetime);

                            _bd.executaComando(sqlInsertSport);

                            string sqlUpdateData = @"update SportRecord set _datahora = dateadd(hour, -3, DATEADD(s, cast([time] as int), '1970-01-01 00:00:00')) where _datahora is null";
                            _bd.executaComando(sqlUpdateData);
                        }
                    }

                    lineNumber++;
                }
            }

            MessageBox.Show("terminou!!");

            #region arquivo fitness
            //using (TextFieldParser parser = new TextFieldParser(caminhoBase + arquivoFitness))
            //{
            //    parser.TextFieldType = FieldType.Delimited;
            //    parser.SetDelimiters(",");

            //    int lineNumber = 1;

            //    while (!parser.EndOfData)
            //    {
            //        string[] fields = parser.ReadFields();
            //        if (lineNumber > 1)
            //        {
            //            string uid = fields[0];
            //            string sid = fields[1];
            //            string key = fields[2];
            //            string time = fields[3];
            //            string value = fields[4];
            //            string updatetime = fields[5];

            //            //string sqlConsultaSport = string.Format("select 1 from fitnessdata " +
            //            //    " where [key] = '{0}' and time = '{1}' and value = '{2}' ",
            //            //    key, time, value);

            //            //DataTable dt = _bd.listar(sqlConsultaSport).Tables[0];
            //            //if (dt.Rows.Count <= 0)
            //            //{

            //            string sqlInsertSport = @"insert into fitnessdata (Uid,Sid,[Key],Time,Value,UpdateTime) ";
            //            sqlInsertSport += string.Format("values ('{0}','{1}','{2}','{3}','{4}','{5}')",
            //                uid, sid, key, time, value, updatetime);

            //            _bd.executaComando(sqlInsertSport);

            //            //}
            //        }

            //        lineNumber++;
            //    }

            //    MessageBox.Show("terminou!!");
            //}
            #endregion arquivo fitness
        }

        private void btnGerarJsonStrava_Click(object sender, EventArgs e)
        {
            AcessoDados _bd = new AcessoDados();

            //string sql = @"update SportRecord set _datahora = DATEADD(s, cast([time] as int), '1970-01-01 00:00:00') where _datahora is null";
            //_bd.executaComando(sql);

            string sqlConsulta = @"select id, [value], [key], _datahora _datahora
                        From SportRecord
                        where _datahora >= '2024-06-01'
                        and StravaId is null
                        and [key]  not like 'outdoor%'";

            DataTable dt = _bd.listar(sqlConsulta).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                string jsonValue = dr["value"].ToString();

                dynamic json = JsonConvert.DeserializeObject(jsonValue);
                int duration = Convert.ToInt32(json.duration.ToString());
                int distance = json.distance == null ? 0 : string.IsNullOrEmpty(json.distance.ToString()) ? 0 : Convert.ToInt32(json.distance.ToString());

                string idMiFitness = dr["id"].ToString();

                string _datahora = dr["_datahora"].ToString();
                DateTime _datahoraConverted = Convert.ToDateTime(_datahora, new CultureInfo("pt-br"));

                var options = new RestClientOptions("https://www.strava.com")
                {
                    MaxTimeout = -1,
                    FollowRedirects = false,
                };

                string sportStrava = string.Empty;
                string nomeStrava = string.Empty;

                switch (dr["key"].ToString())
                {
                    case "indoor_running":
                        sportStrava = "VirtualRun";
                        nomeStrava = "esteira";
                        break;

                    case "indoor_walking":
                        sportStrava = "Walk";
                        nomeStrava = "caminhada na esteira";
                        break;

                    //case "outdoor_running":
                    //    sportStrava = "Run";
                    //    nomeStrava = "corrida";
                    //    break;

                    case "indoor_fitness":
                        sportStrava = "WeightTraining";
                        nomeStrava = "musculação";
                        break;

                    case "climbing_machine":
                        sportStrava = "StairStepper";
                        nomeStrava = "escada";
                        break;

                    case "elliptical_trainer":
                        sportStrava = "Elliptical";
                        nomeStrava = "Eliptico";
                        break;
                }

                string tokenAuth = "";
                if (!string.IsNullOrEmpty(sportStrava))
                {
                    var client = new RestClient(options);
                    var request = new RestRequest("/api/v3/activities", Method.Post);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddHeader("Authorization", "Bearer " + tokenAuth);
                    request.AddParameter("name", nomeStrava);
                    request.AddParameter("type", sportStrava);
                    request.AddParameter("sport_type", sportStrava);
                    request.AddParameter("start_date_local", _datahoraConverted.ToString("yyyy-MM-ddTHH\\:mm\\:ssZ"));
                    request.AddParameter("elapsed_time", duration);
                    request.AddParameter("description", "");
                    request.AddParameter("distance", distance);

                    RestResponse response = client.Execute(request);
                    Console.WriteLine(response.Content);

                    if (response.IsSuccessful)
                    {
                        dynamic jsonStrava = JsonConvert.DeserializeObject(response.Content);
                        string stravaId = jsonStrava.id.ToString();

                        string sqlUpdate = @"UPDATE SportRecord SET StravaId = '" + stravaId + "' WHERE ID = " + idMiFitness;
                        _bd.executaComando(sqlUpdate);

                        var client2 = new RestClient(options);
                        var request2 = new RestRequest("/api/v3/activities/" + stravaId, Method.Put);
                        request2.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        request2.AddHeader("Authorization", "Bearer " + tokenAuth);
                        request2.AddParameter("hide_from_home", "1");
                        RestResponse response2 = client.Execute(request2);
                        Console.WriteLine(response2.Content);

                        if (response2.IsSuccessful)
                        {

                        }
                        else
                        {
                            // gravar log do erro
                        }
                    }
                    else
                    {
                        // gravar log do erro
                    }
                }
                else
                {
                }
            }
        }
    }
}