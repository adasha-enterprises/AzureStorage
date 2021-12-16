using Microsoft.AspNetCore.Mvc;
using SupervisorService.Interfaces;
using QueueStorageLibrary.Models;
using System;
using System.Threading.Tasks;

namespace SupervisorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupervisorController : Controller
    {
        private readonly ISupervisorService _storageService;

        /// <summary>
        /// Constructor or Supervisor Controller
        /// </summary>
        /// <param name="storageService"></param>
        public SupervisorController(ISupervisorService storageService)
        {
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        /// <summary>
        /// Default page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName(nameof(Index))]
        public IActionResult Index()
        {
            return Ok(new { Status = "Awaiting Orders!" });
        }

        /// <summary>
        /// Get order confirmation
        /// </summary>
        /// <param name="id">The id of the order confirmation to retrieve</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ActionName(nameof(GetAsync))]
        public async Task<IActionResult> GetAsync(string id)
        {
            return Ok(await _storageService.RetrieveAsync(id));
        }

        /// <summary>
        /// Add order to be processed
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName(nameof(PostAsync))]
        public async Task<IActionResult> PostAsync([FromBody] OrderDTO entity)
        {
            var createdEntity = await _storageService.InsertOrderAsync(entity);

            return CreatedAtAction(nameof(PostAsync), createdEntity);
        }
    }
}
