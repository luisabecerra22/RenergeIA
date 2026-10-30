# Abre un .docx con Word en segundo plano, actualiza el índice y guarda. Opcional: exporta a PDF.
# Uso: powershell -File actualizar-indice.ps1 -Archivo <archivo.docx> [-Pdf <salida.pdf>]
# Nunca cierra un Word que tenga otros documentos abiertos (el de la usuaria).
param([string]$Archivo, [string]$Pdf)
$w = New-Object -ComObject Word.Application
$w.DisplayAlerts = 0
try {
  $d = $w.Documents.Open($Archivo, $false, $false, $false)
  foreach ($t in $d.TablesOfContents) { $t.Update() }
  $d.Save()
  if ($Pdf) { $d.ExportAsFixedFormat($Pdf, 17) }
  $d.Close(0)
} catch {
  # Word por COM se desconecta a veces DESPUÉS de guardar; el archivo queda bien
  Write-Output ("Aviso: " + $_.Exception.Message)
}
try { if ($w.Documents.Count -eq 0) { $w.Quit() } } catch {}
