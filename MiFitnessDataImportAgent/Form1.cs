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
            string caminhoBase = @"C:\Users\dssca\Downloads\xiaomi-data-mifitness-20240521\20240522_6599729986_MiFitness_c3_data_copy-f1\";
            string arquivoSport = @"20240522_6599729986_MiFitness_hlth_center_sport_record.csv";
            string arquivoFitness = @"20240522_6599729986_MiFitness_hlth_center_fitness_data.csv";

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

                        string sqlInsertSport = @"insert into sportrecord (Uid,Sid,[Key],Time,Category,Value,UpdateTime) ";
                        sqlInsertSport += string.Format("values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                            uid, sid, key, time, category, value, updatetime);

                        _bd.executaComando(sqlInsertSport);
                    }

                    lineNumber++;
                }
            }

            using (TextFieldParser parser = new TextFieldParser(caminhoBase + arquivoFitness))
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
                        string value = fields[4];
                        string updatetime = fields[5];

                        string sqlInsertSport = @"insert into fitnessdata (Uid,Sid,[Key],Time,Value,UpdateTime) ";
                        sqlInsertSport += string.Format("values ('{0}','{1}','{2}','{3}','{4}','{5}')",
                            uid, sid, key, time, value, updatetime);

                        _bd.executaComando(sqlInsertSport);
                    }

                    lineNumber++;
                }
            }
        }

        private void btnGerarJsonStrava_Click(object sender, EventArgs e)
        {
            AcessoDados _bd = new AcessoDados();

            string sql = @"update SportRecord set _datahora = DATEADD(s, cast([time] as int), '1970-01-01 00:00:00') where _datahora is null";
            _bd.executaComando(sql);

            string sqlConsulta = @"select id, [value], [key], dateadd(hour,-3,_datahora) _datahora
                        From SportRecord
                        where _datahora >= '2024-05-01'
                        and StravaId is null";


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

                    case "indoor_fitness":
                        sportStrava = "WeightTraining";
                        nomeStrava = "musculação";
                        break;

                    case "climbing_machine":
                        sportStrava = "StairStepper";
                        nomeStrava = "escada";
                        break;
                }

                if (!string.IsNullOrEmpty(sportStrava))
                {

                    var client = new RestClient(options);
                    var request = new RestRequest("/api/v3/activities", Method.Post);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddHeader("Authorization", "Bearer ");
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