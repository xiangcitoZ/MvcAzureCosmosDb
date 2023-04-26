using Microsoft.AspNetCore.Mvc;
using MvcAzureCosmosDb.Models;
using MvcAzureCosmosDb.Services;

namespace MvcAzureCosmosDb.Controllers
{
    public class CochesController : Controller
    {
        private ServiceCosmosDb service;

        public CochesController(ServiceCosmosDb service)
        {
            this.service = service;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string accion)
        {
            await this.service.CreateDatabaseAsync();
            ViewData["MENSAJE"] = "Recursos creados en Azure Cosmos Db";
            return View();


        }

        public async Task<IActionResult> Vehiculos()
        {
            List<Vehiculo> cars = 
                await this.service.GetVehiculosAsync();
            return View(cars);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Vehiculo car)
        {
            await this.service.InsertVehiculoAsync(car);
            return RedirectToAction("Vehiculos");
        }


    }
}
