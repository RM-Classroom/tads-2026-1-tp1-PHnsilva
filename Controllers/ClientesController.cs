using LocadoraVeiculosApi.Infrastructure.Data;
using LocadoraVeiculosApi.Application.DTOs;
using LocadoraVeiculosApi.Shared.Helpers;
using LocadoraVeiculosApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ApplicationContext _context;

    public ClientesController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll()
    {
        var clientes = await _context.Clientes
            .OrderBy(x => x.Nome)
            .Select(x => new ClienteDto
            {
                Id = x.Id,
                Nome = x.Nome,
                Cpf = x.Cpf,
                Email = x.Email,
                Telefone = x.Telefone
            })
            .ToListAsync();

        return Ok(clientes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteDto>> GetById(int id)
    {
        var cliente = await _context.Clientes
            .Where(x => x.Id == id)
            .Select(x => new ClienteDto
            {
                Id = x.Id,
                Nome = x.Nome,
                Cpf = x.Cpf,
                Email = x.Email,
                Telefone = x.Telefone
            })
            .FirstOrDefaultAsync();

        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpPost]
    public async Task<ActionResult<ClienteDto>> Create(ClienteDto dto)
    {
        var cliente = new Cliente
        {
            Nome = dto.Nome,
            Cpf = dto.Cpf,
            Email = dto.Email,
            Telefone = dto.Telefone
        };

        _context.Clientes.Add(cliente);

        try
        {
            await _context.SaveChangesAsync();
            dto.Id = cliente.Id;
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        catch (DbUpdateException ex) when (DbExceptionHelper.IsUniqueConstraint(ex))
        {
            return Conflict(new { erro = "CPF ou e-mail já cadastrados." });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ClienteDto dto)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente is null) return NotFound();

        cliente.Nome = dto.Nome;
        cliente.Cpf = dto.Cpf;
        cliente.Email = dto.Email;
        cliente.Telefone = dto.Telefone;

        try
        {
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex) when (DbExceptionHelper.IsUniqueConstraint(ex))
        {
            return Conflict(new { erro = "CPF ou e-mail já cadastrados." });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente is null) return NotFound();

        _context.Clientes.Remove(cliente);

        try
        {
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex) when (DbExceptionHelper.IsForeignKeyConstraint(ex))
        {
            return Conflict(new { erro = "Não é possível excluir o cliente porque existem aluguéis vinculados a ele." });
        }
    }
}
