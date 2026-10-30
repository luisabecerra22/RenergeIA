# Genera los Word sobre la plantilla, actualiza los índices con Word y les da el formato del índice de la plantilla.
# Uso: powershell -File generar.ps1 [-Plantilla <ruta .docx>]
param([string]$Plantilla)
$aqui = Split-Path -Parent $MyInvocation.MyCommand.Path
$salida = Join-Path (Split-Path -Parent $aqui) 'word'
$build = Join-Path $aqui 'build.js'

if ($Plantilla) { node $build $Plantilla } else { node $build }

foreach ($f in Get-ChildItem $salida -Filter *.docx) {
  & (Join-Path $aqui 'actualizar-indice.ps1') -Archivo $f.FullName
  node $build --indice $f.FullName
}
