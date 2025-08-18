using OpenFga.Sdk.Client;
using OpenFga.Sdk.Client.Model;
using OpenFga.Sdk.Model;
using System.Text.Json;

namespace Service.openFga
{
    public class OpenFgaConfig
    {
        public string Url { get; set; } = "http://openfga:8080";
        public string StoreId { get; set; } = "01K2YZ60WMJ8SAEZR4ZKAY7V1R";
        public string AuthModelId { get; set; }

        public OpenFgaClient fgaClient { get; set; }


        public async Task Init()
        {
            var configuration = new ClientConfiguration()
            {
                ApiUrl = this.Url,
                StoreId = this.StoreId,
            };
            this.fgaClient = new OpenFgaClient(configuration);

            var modelJson = "{\"schema_version\":\"1.1\",\"type_definitions\":[{\"type\":\"user\"},{\"type\":\"document\",\"relations\":{\"ro\":{\"this\":{}},\"rw\":{\"this\":{}},\"ow\":{\"this\":{}}},\"metadata\":{\"relations\":{\"ro\":{\"directly_related_user_types\":[{\"type\":\"user\"}]},\"rw\":{\"directly_related_user_types\":[{\"type\":\"user\"}]},\"ow\":{\"directly_related_user_types\":[{\"type\":\"user\"}]}}}}]}";
            var body = JsonSerializer.Deserialize<OpenFga.Sdk.Client.Model.ClientWriteAuthorizationModelRequest>(modelJson);

            var response = await fgaClient.WriteAuthorizationModel(body!);
            this.AuthModelId = response.AuthorizationModelId.ToString();

        }

        public async Task CreateTuple(string name, string obj, string relation) //futuramente, ao criar um usuario no banco de dados, automaticamente vai criar seu tuple. 
                                                                                //Receber UserModel
        {
            if (this.AuthModelId == null)
            {
                await Init();
            }
            var options = new ClientWriteOptions()
            {
                AuthorizationModelId = this.AuthModelId
            };

            var body = new ClientWriteRequest()
            {
                Writes = new List<ClientTupleKey>() {
                new()
                    {
                        User = $"user:{name}",
                        Object = $"document:{obj}",
                        Relation = relation
                    }
                }
            };

            var response = await this.fgaClient.Write(body, options);
        }

        public async Task<CheckResponse?> CheckTuple(string name, string obj, string relat) //futuramente receber um UserMOdel com relação Cargo, mas pra isso o authModel precisa de Group
        {
            var options = new ClientCheckOptions
            {
                AuthorizationModelId = this.AuthModelId
            };

            var body = new ClientCheckRequest
            {
                User = $"user:{name}",
                Relation = relat,
                Object = $"document:{obj}"
            };

            var response = await fgaClient.Check(body, options);

            return response;

        }
    

    }
}
