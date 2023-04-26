using Microsoft.Azure.Cosmos;
using MvcAzureCosmosDb.Models;

namespace MvcAzureCosmosDb.Services
{
    public class ServiceCosmosDb
    {
        //SE TRABAJA CON ITEMS CONTAINERS
        //DENTRO DE CONTAINER PODEMOS RECUPERAR EL CONTAINER
        //SE UTILIZA UNA CLASE CLIENT PARA RECUPERAR ELEMENTOS DE COSMOS
        //LLAMADA CosmosClient
        private CosmosClient client;
        public Container containerCosmos;

        public ServiceCosmosDb(CosmosClient client,
            Container container)
        {
            this.client = client;
            this.containerCosmos = container;
        }

        //TENDREMOS UN METODO PARA CREAR LA BASE DE DATOS
        //Y DENTRO UN CONTENEDOR
        public async Task CreateDatabaseAsync()
        {
            //HEMOS DICHO QUE LA PRIMARY KEY SERA id
            //PERO PODRIAMOS INDICAR, DE FORMA EXPLICITA 
            //QUE DICHA PRIMARY KEY SERA OTRA
            ContainerProperties properties =
                new ContainerProperties("containercoches", "/id");
            //ESTO ES ITEMS CONTAINERS
            await this.client.CreateDatabaseIfNotExistsAsync
                ("vehiculoscosmos");
            //CREAMOS UN NUEVO CONTENEDOR DENTRO DE ITEMS CONTAINERS
            await this.client.GetDatabase("vehiculoscosmos")
                .CreateContainerIfNotExistsAsync(properties);
        }

        //METODO PARA INSERTAR ITEMS EN NUESTRO CONTAINER
        public async Task InsertVehiculoAsync(Vehiculo car)
        {
            //EN EL MOMENTO DE CREAR UN NUEVO ITEM
            //DEBEMOS INDICAR EL ITEM Y TAMBIEN SU PARTITION KEY
            //DE FORMA EXPLICITA
            await this.containerCosmos.CreateItemAsync<Vehiculo>
                (car, new PartitionKey(car.Id));
        }

        //METODO PARA RECUPERAR TODOS LOS VEHICULOS
        public async Task<List<Vehiculo>> GetVehiculosAsync()
        {
            //LOS DATOS SE RECUPERAN MEDIANTE Iterator
            //NECESITAMOS RECORRER LOS ITEMS MIENTRAS QUE EXISTAN
            var query =
                this.containerCosmos.GetItemQueryIterator<Vehiculo>();
            List<Vehiculo> coches = new List<Vehiculo>();
            while (query.HasMoreResults)
            {
                var results = await query.ReadNextAsync();
                //AÑADIMOS LOS COCHES QUE IRA RECUPERANDO DENTRO 
                //DE LA COLECCION
                coches.AddRange(results);
            }
            return coches;
        }

        //METODO PARA MODIFICAR UN VEHICULO
        public async Task UpdateVehiculoAsync(Vehiculo car)
        {
            //TENEMOS UN METODO QUE ES Upsert QUE ES UNA MEZCLA
            //ENTRE UPDATE E INSERT.
            //SI LO ENCUENTRA, LO MODIFICA, SI NO LO ENCUENTRA LO INSERTA
            await this.containerCosmos.UpsertItemAsync<Vehiculo>
                (car, new PartitionKey(car.Id));
        }

        //METODO PARA ELIMINAR UN VEHICULO
        //EN DICHO METODO NECESITARIAMOS PARTITION KEY SI 
        //FUERA DISTINTA DEL CAMPO ID
        public async Task DeleteVehiculoAsync(string id)
        {
            await this.containerCosmos.DeleteItemAsync<Vehiculo>
                (id, new PartitionKey(id));
        }

        //METODO PARA BUSCAR UN VEHICULO
        //EN DICHO METODO NECESITARIAMOS PARTITION KEY
        //EN EL CASO QUE NO FUERAN IGUALES
        public async Task<Vehiculo> FindVehiculoAsync(string id)
        {
            ItemResponse<Vehiculo> response =
                await this.containerCosmos.ReadItemAsync<Vehiculo>
                (id, new PartitionKey(id));
            return response.Resource;
        }




    }
}
