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

            var modelJson = "{\"schema_version\":\"1.1\",\"type_definitions\":[{\"type\":\"papel\",\"relations\":{\"rw\":{\"this\":{}},\"ro\":{\"this\":{}}},\"metadata\":{\"relations\":{\"rw\":{\"directly_related_user_types\":[{\"type\":\"user\"}]},\"ro\":{\"directly_related_user_types\":[{\"type\":\"user\"}]}}}},{\"type\":\"user\",\"relations\":{},\"metadata\":null},{\"type\":\"endpoint\",\"relations\":{\"papel\":{\"this\":{}},\"accessible\":{\"union\":{\"child\":[{\"this\":{}},{\"tupleToUserset\":{\"computedUserset\":{\"relation\":\"rw\"},\"tupleset\":{\"relation\":\"papel\"}}},{\"tupleToUserset\":{\"computedUserset\":{\"relation\":\"ro\"},\"tupleset\":{\"relation\":\"papel\"}}}]}}},\"metadata\":{\"relations\":{\"papel\":{\"directly_related_user_types\":[{\"type\":\"papel\"}]},\"accessible\":{\"directly_related_user_types\":[{\"type\":\"user\"}]}}}}]}";
            var body = JsonSerializer.Deserialize<OpenFga.Sdk.Client.Model.ClientWriteAuthorizationModelRequest>(modelJson);

            var response = await fgaClient.WriteAuthorizationModel(body!);
            this.AuthModelId = response.AuthorizationModelId.ToString();

        }

        public async Task CreateTuple(string name, string obj, string relation)
                                                                                
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
                        User = name,
                        Object = obj,
                        Relation = relation
                    }
                }
            };

            var response = await this.fgaClient.Write(body, options);
        }

        public async Task<Boolean> CheckTuple(string name, string obj, string relat)
        {
            var options = new ClientCheckOptions
            {
                AuthorizationModelId = this.AuthModelId
            };

            var body = new ClientCheckRequest
            {
                User = $"user:{name}",
                Relation = relat,
                Object = obj
            };

            var response = await fgaClient.Check(body, options);

         

            return response.Allowed == true;

        }
    

    }
}
