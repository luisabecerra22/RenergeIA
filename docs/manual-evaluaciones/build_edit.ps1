$ErrorActionPreference = "Stop"
$base = "C:\Users\Luisa Becerra\Downloads\RenergeIA\docs\manual-evaluaciones"
$src  = "C:\Users\Luisa Becerra\Downloads\IN-SG-SS-007-1.docx"
$out  = Join-Path $base "IN-SG-SS-007-1.docx"
$pdf  = Join-Path $base "IN-SG-SS-007-1.pdf"
$imgDir = Join-Path $base "img\final"
$jsonPath = Join-Path $base "content.json"

$cfg = [System.IO.File]::ReadAllText($jsonPath, [System.Text.Encoding]::UTF8) | ConvertFrom-Json

Copy-Item $src $out -Force
try { Unblock-File $out } catch {}

$word = New-Object -ComObject Word.Application
$word.Visible = $false
$word.DisplayAlerts = 0

function StyleObj($doc, $t) {
  $cands = @()
  switch ($t) {
    'h1'   { $cands = @('Titulo 1','Título 1',-2) }
    'h2'   { $cands = @('Titulo 2','Título 2',-3) }
    'body' { $cands = @('Normal',-1) }
    'bul'  { $cands = @('Parrafo de lista','Párrafo de lista',-63) }
    default { $cands = @('Normal',-1) }
  }
  foreach ($c in $cands) { try { return $doc.Styles.Item($c) } catch {} }
  return $doc.Styles.Item(-1)
}

function ScaleShape($shape, $maxW, $maxH) {
  $shape.LockAspectRatio = -1
  if ($shape.Width -gt $maxW) { $shape.Width = $maxW }
  if ($shape.Height -gt $maxH) { $shape.Height = $maxH }
}

function FindPara($doc, $text) {
  $rng = $doc.Content
  $f = $rng.Find
  $f.ClearFormatting()
  $f.Forward = $true
  $f.Wrap = 0
  $f.MatchCase = $false
  $f.Text = $text
  $ok = $f.Execute()
  if ($ok) { return $rng.Paragraphs.Item(1) } else { return $null }
}

function InsertAfterPara($doc, $word, $para, $items, $imgDir) {
  $sel = $word.Selection
  $pos = $para.Range.End - 1
  $null = $sel.SetRange($pos, $pos)
  $sel.TypeParagraph()
  for ($i = 0; $i -lt $items.Count; $i++) {
    if ($i -gt 0) { $sel.TypeParagraph() }
    $it = $items[$i]
    try { $sel.Range.ListFormat.RemoveNumbers() } catch {}
    if ($it.t -eq 'img') {
      $sel.Style = StyleObj $doc 'body'
      $sel.ParagraphFormat.Alignment = 1
      $p = Join-Path $imgDir $it.x
      $shape = $sel.InlineShapes.AddPicture($p, $false, $true)
      ScaleShape $shape 440 340
      $sel.Collapse(0)
    } else {
      $sel.Style = StyleObj $doc $it.t
      if ($it.t -eq 'body' -or $it.t -eq 'bul') { $sel.ParagraphFormat.Alignment = 3 }
      $sel.TypeText($it.x)
      if ($it.t -eq 'bul') { try { $sel.Range.ListFormat.ApplyBulletDefault() } catch {} }
    }
  }
}

$log = @()
try {
  $doc = $word.Documents.Open($out, $false, $false)

  # 1) Reemplazar marcadores "/" con imagenes
  $phParas = @()
  foreach ($p in $doc.Paragraphs) {
    if ($p.Range.Text.Trim() -eq '/') { $phParas += $p }
  }
  $log += "placeholders encontrados: $($phParas.Count)"
  $n = [Math]::Min($cfg.placeholders.Count, $phParas.Count)
  for ($i = 0; $i -lt $n; $i++) {
    $p = $phParas[$i]
    $r = $p.Range
    $r.End = $r.End - 1
    $r.Text = ''
    $img = Join-Path $imgDir $cfg.placeholders[$i]
    $shape = $r.InlineShapes.AddPicture($img, $false, $true)
    ScaleShape $shape 460 380
    $p.Alignment = 1
  }
  $log += "imagenes de marcadores insertadas: $n"

  # 2) Inserciones de bloques
  foreach ($ins in $cfg.inserts) {
    if ($ins.mode -eq 'after') {
      $anchor = FindPara $doc $ins.anchor
      if ($anchor -ne $null) { InsertAfterPara $doc $word $anchor $ins.items $imgDir; $log += "insert after OK: $($ins.anchor.Substring(0,[Math]::Min(30,$ins.anchor.Length)))" }
      else { $log += "ANCHOR NO ENCONTRADO (after): $($ins.anchor)" }
    } elseif ($ins.mode -eq 'beforeHeading') {
      $h = FindPara $doc $ins.anchor
      if ($h -ne $null) {
        $before = $doc.Range(0, $h.Range.Start - 1)
        $prev = $before.Paragraphs.Item($before.Paragraphs.Count)
        InsertAfterPara $doc $word $prev $ins.items $imgDir
        $log += "insert beforeHeading OK: $($ins.anchor)"
      } else { $log += "ANCHOR NO ENCONTRADO (beforeHeading): $($ins.anchor)" }
    }
  }

  # 3) Renombrar el capitulo final mal etiquetado
  $rp = FindPara $doc $cfg.rename.findBefore
  if ($rp -ne $null) {
    $before = $doc.Range(0, $rp.Range.Start - 1)
    $head = $before.Paragraphs.Item($before.Paragraphs.Count)
    $hr = $head.Range
    $hr.End = $hr.End - 1
    $hr.Text = $cfg.rename.to
    $log += "renombrado capitulo final: $($cfg.rename.to)"
  } else { $log += "rename anchor no encontrado" }

  # 4) Actualizar indice (TOC) y campos
  try { if ($doc.TablesOfContents.Count -gt 0) { $doc.TablesOfContents.Item(1).Update() } } catch { $log += "TOC update err: $($_.Exception.Message)" }
  try { $doc.Fields.Update() } catch {}

  # 5) Guardar y exportar
  $doc.SaveAs2($out, 16)
  $doc.ExportAsFixedFormat($pdf, 17)
  $pages = $doc.ComputeStatistics(2)
  $doc.Close($false)
  $log += "GUARDADO. paginas=$pages docx=$(Test-Path $out) pdf=$(Test-Path $pdf)"
} catch {
  $log += "ERROR: $($_.Exception.Message)"
} finally {
  $word.Quit()
}
$log -join "`n"
