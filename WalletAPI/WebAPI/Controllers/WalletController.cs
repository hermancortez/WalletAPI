using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        /// <summary>
        /// Obtiene todas las billeteras existentes.
        /// </summary>
        /// <returns>Lista de billeteras.</returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WalletDto>>> GetWallets()
        {
            var wallets = await _walletService.GetAllWalletsAsync();
            return Ok(wallets);
        }

        /// <summary>
        /// Obtiene una billetera específica por su Id.
        /// </summary>
        /// <param name="id">Id de la billetera.</param>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<WalletDto>> GetWallet(int id)
        {
            var wallet = await _walletService.GetWalletByIdAsync(id);
            if (wallet == null)
                return NotFound(new { Message = $"Billetera con id {id} no existe." });

            return Ok(wallet);
        }


        /// <summary>
        /// Crea una nueva billetera.
        /// </summary>
        /// <param name="walletDto">Datos necesarios para la creación.</param>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<WalletDto>> CreateWallet(CreateWalletDto walletDto)
        {
            var result = await _walletService.CreateWalletAsync(walletDto);
            if (!result.Success)
                return BadRequest(new { result.Message });

            if (result.Wallet == null)
            {
                return BadRequest(new { Message = "Error al crear la billetera." });
            }

            return CreatedAtAction(nameof(GetWallet), new { id = result.Wallet.Id }, result.Wallet);
        }

        /// <summary>
        /// Actualiza el nombre de una billetera existente.
        /// </summary>
        /// <param name="id">Id de la billetera a actualizar.</param>
        /// <param name="walletDto">Nuevo nombre para la billetera.</param>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWallet(int id, UpdateWalletDto walletDto)
        {
            var result = await _walletService.UpdateWalletAsync(id, walletDto);
            if (!result.Success)
                return NotFound(new { Message = result.Message });

            return NoContent();
        }

        /// <summary>
        /// Elimina una billetera existente.
        /// </summary>
        /// <param name="id">Id de la billetera a eliminar.</param>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWallet(int id)
        {
            var result = await _walletService.DeleteWalletAsync(id);

            if (!result.Success)
                return NotFound(new { Message = result.Message });

            return NoContent();
        }

        /// <summary>
        /// Realiza una transferencia entre dos billeteras.
        /// </summary>
        /// <param name="transferDto">Datos para realizar la transferencia.</param>
        [Authorize]
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(TransferDto transferDto)
        {
            var result = await _walletService.TransferAsync(transferDto.SourceWalletId, transferDto.TargetWalletId, transferDto.Amount);

            if (!result.Success)
                return BadRequest(new { result.Message });

            return Ok(new { result.Message });
        }

        /// <summary>
        /// Obtiene el historial de movimientos de una billetera con paginación y filtrado por fechas.
        /// </summary>
        [HttpGet("{walletId}/transactions")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions(int walletId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var transactions = await _walletService.GetTransactionsByWalletAsync(walletId, fromDate, toDate, page, pageSize);
            return Ok(transactions);
        }
    }

}
