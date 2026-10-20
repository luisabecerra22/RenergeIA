namespace RenergeIA.Core.Entities;

// Lote de importación de Actas IA: deja respaldo de lo creado/modificado para poder revertir.
public class LoteImportacionActas : EntidadBase
{
    public string Archivo { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string? ResumenJson { get; set; }    // resultado de la importación (conteos y conflictos)
    public string? RespaldoJson { get; set; }   // ids creados + snapshots de lo modificado, para revertir
    public bool Revertido { get; set; }
    public DateTime? RevertidoEl { get; set; }
}
