using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculosApi.Shared.Helpers;

public static class DbExceptionHelper
{
    public static bool IsUniqueConstraint(DbUpdateException ex)
    {
        var message = ex.InnerException?.Message ?? ex.Message;
        var normalized = message.ToLowerInvariant();
        return normalized.Contains("duplicate")
            || normalized.Contains("unique")
            || normalized.Contains("ix_clientes_cpf")
            || normalized.Contains("ix_clientes_email")
            || normalized.Contains("ix_veiculos_placa");
    }

    public static bool IsForeignKeyConstraint(DbUpdateException ex)
    {
        var message = ex.InnerException?.Message ?? ex.Message;
        return message.ToLowerInvariant().Contains("foreign key");
    }
}
