# Prompts para las imágenes de la Oficina RenergeIA (estilo Pixar)

Imágenes necesarias: 6 personajes × 2 poses + 1 oficina vacía = **13 PNG**.
Referencia de estilo: la imagen de la oficina RenergeIA estilo Pixar generada el 2026-09-25.

## Reglas para generar (léelas antes)

1. **Genera primero la pose NORMAL** de cada personaje. Cuando quede bien, **adjunta esa misma imagen** y usa el prompt de MANO LEVANTADA, que le pide *editar* la imagen sin mover nada más. Así las dos poses quedan alineadas.
2. Todos los personajes en lienzo **cuadrado 1024×1024, fondo transparente real** (PNG con alpha, no un cuadriculado dibujado). Si la herramienta no soporta transparencia, pide fondo **verde puro (#00FF00)** y avísame: lo recorto yo.
3. El personaje debe ocupar la misma zona en todos: cabeza empezando ~15 % desde arriba, pies ~88 % desde arriba, centrado. Cuerpo completo, de pie, mirando al frente, sin escritorio, sin sombra, sin texto.
4. Deja espacio libre arriba y a los lados para que la mano levantada quepa en el mismo lienzo.
5. Si un personaje sale muy distinto a la referencia, vuelve a generar la pose normal; **no aceptes una mano levantada que cambie la cara, la ropa o el tamaño**.
6. Guarda con estos nombres exactos:

| Agente | Normal | Mano levantada |
|---|---|---|
| Costos | `costos-normal.png` | `costos-mano-levantada.png` |
| Documental | `documental-normal.png` | `documental-mano-levantada.png` |
| HSEQ | `hseq-normal.png` | `hseq-mano-levantada.png` |
| Planeación | `planeacion-normal.png` | `planeacion-mano-levantada.png` |
| Seguimiento | `seguimiento-normal.png` | `seguimiento-mano-levantada.png` |
| Administrativo | `administrativo-normal.png` | `administrativo-mano-levantada.png` |
| Calidad | `calidad-normal.png` | `calidad-mano-levantada.png` |
| Oficina vacía | `oficina-fondo.png` | — |

---

## Bloque de estilo común (va al inicio de TODOS los prompts de personajes)

```
Pixar-style 3D animated character, same visual style as the RenergeIA office reference image: soft studio lighting, smooth stylized skin, big expressive eyes, warm friendly look, clean corporate outfit. Full-body, standing, facing the camera, camera slightly elevated. Square 1024x1024 PNG with a genuinely TRANSPARENT background (real alpha, not a drawn checkerboard). Character centered horizontally, top of head at about 15% from the top edge, feet baseline at about 88% from the top edge. Leave empty space above the head and on both sides. No floor, no shadow, no desk, no props on the ground, no text, no logo, no frame. One character, one pose.
```

---

## 1. COSTOS — mujer joven de moño, blusa crema (la de la izquierda en la referencia)

### 1a. Normal
```
[BLOQUE DE ESTILO COMÚN]
Character: a young Latina woman in her late twenties, brown hair in a neat high bun with a few loose strands, warm brown eyes, light freckles, friendly confident smile. Outfit: cream short-sleeve blouse, dark navy (#183963) slim trousers, a green (#6ABF4B) lanyard with a badge, small stud earrings, flat dark shoes. Holds a small navy tablet against her chest with her left hand; right arm relaxed DOWN at her side. Neutral idle pose, weight evenly on both feet.
```

### 1b. Mano levantada (adjuntar `costos-normal.png`)
```
Edit the attached image. Keep EXACTLY the same character: same face, hair bun, freckles, cream blouse, navy trousers, green lanyard, tablet in the left hand, same body position, same size, same feet baseline, same 1024x1024 canvas and real transparent background. Change ONLY her right arm: raise it beside her head, elbow bent, open palm facing forward, as if politely asking for attention. Give her a slightly more eager expression with raised eyebrows. Nothing else moves. No shadow, no text, no background.
```

## 2. DOCUMENTAL — hombre mayor de gafas y pelo gris (el del cardigan morado)

### 2a. Normal
```
[BLOQUE DE ESTILO COMÚN]
Character: a distinguished man in his late fifties, short neatly combed gray hair, thin dark-rimmed rectangular glasses, kind attentive expression with a small smile, clean-shaven. Outfit: dark plum cardigan over a white collared shirt and a navy (#183963) tie, charcoal trousers, brown leather shoes. Holds a green (#6ABF4B) document folder under his left arm; right arm relaxed DOWN at his side. Neutral idle pose, weight evenly on both feet.
```

### 2b. Mano levantada (adjuntar `documental-normal.png`)
```
Edit the attached image. Keep EXACTLY the same character: same face, gray hair, glasses, plum cardigan, white shirt, navy tie, green folder under the left arm, same body position, same size, same feet baseline, same 1024x1024 canvas and real transparent background. Change ONLY his right arm: raise it beside his head, elbow bent, open palm facing forward, as if politely asking for attention. Slightly raise his eyebrows. Nothing else moves. No shadow, no text, no background.
```

## 3. HSEQ — mujer de pelo rizado con camisa verde (la del centro)

### 3a. Normal
```
[BLOQUE DE ESTILO COMÚN]
Character: an Afro-Latina woman in her early thirties, voluminous dark curly hair, warm brown skin, bright expressive eyes, big welcoming smile. Outfit: bright green (#6ABF4B) polo shirt with a small white RenergeIA-style leaf emblem on the chest, dark navy (#183963) trousers, a white safety helmet held in her left hand at hip level, a navy lanyard with a badge, sturdy dark work shoes. Right arm relaxed DOWN at her side. Neutral idle pose, weight evenly on both feet.
```

### 3b. Mano levantada (adjuntar `hseq-normal.png`)
```
Edit the attached image. Keep EXACTLY the same character: same face, curly hair, green polo, navy trousers, white helmet in the left hand, lanyard, same body position, same size, same feet baseline, same 1024x1024 canvas and real transparent background. Change ONLY her right arm: raise it beside her head, elbow bent, open palm facing forward, as if politely asking for attention. Slightly raise her eyebrows. Nothing else moves. No shadow, no text, no background.
```

## 4. PLANEACIÓN — hombre de gafas y bigote, camisa blanca (el de la derecha, atrás)

### 4a. Normal
```
[BLOQUE DE ESTILO COMÚN]
Character: a man in his forties, short dark brown hair neatly parted, thin mustache, round dark-rimmed glasses, focused but friendly expression. Outfit: crisp white long-sleeve shirt with sleeves rolled to the forearm, navy (#183963) tie, dark gray trousers, brown belt, dark shoes. Holds a rolled-up blueprint/schedule under his left arm; right arm relaxed DOWN at his side. Neutral idle pose, weight evenly on both feet.
```

### 4b. Mano levantada (adjuntar `planeacion-normal.png`)
```
Edit the attached image. Keep EXACTLY the same character: same face, hair, mustache, round glasses, white shirt with rolled sleeves, navy tie, rolled blueprint under the left arm, same body position, same size, same feet baseline, same 1024x1024 canvas and real transparent background. Change ONLY his right arm: raise it beside his head, elbow bent, open palm facing forward, as if politely asking for attention. Slightly raise his eyebrows. Nothing else moves. No shadow, no text, no background.
```

## 5. SEGUIMIENTO — mujer de pelo largo oscuro con blazer (la de la derecha, adelante)

### 5a. Normal
```
[BLOQUE DE ESTILO COMÚN]
Character: a Latina woman in her mid-thirties, long straight dark hair past the shoulders, olive skin, warm brown eyes, calm attentive smile. Outfit: navy (#183963) blazer over a white top, slim dark trousers, small green (#6ABF4B) enamel pin on the lapel, low dark heels. Holds a clipboard with a daily report against her chest with her left hand, a pen clipped to it; right arm relaxed DOWN at her side. Neutral idle pose, weight evenly on both feet.
```

### 5b. Mano levantada (adjuntar `seguimiento-normal.png`)
```
Edit the attached image. Keep EXACTLY the same character: same face, long dark hair, navy blazer, white top, green lapel pin, clipboard in the left hand, same body position, same size, same feet baseline, same 1024x1024 canvas and real transparent background. Change ONLY her right arm: raise it beside her head, elbow bent, open palm facing forward, as if politely asking for attention. Slightly raise her eyebrows. Nothing else moves. No shadow, no text, no background.
```

## 6. ADMINISTRATIVO — hombre de bigote con traje gris (el que explica algo)

### 6a. Normal
```
[BLOQUE DE ESTILO COMÚN]
Character: a cheerful man in his late thirties, short wavy brown hair, full brown mustache, expressive eyebrows, wide friendly smile. Outfit: light gray suit jacket over a pale blue shirt, navy (#183963) tie, dark trousers, dark shoes, a green (#6ABF4B) lanyard with a badge. Holds a white coffee mug with a small green leaf logo in his left hand at chest level; right arm relaxed DOWN at his side. Neutral idle pose, weight evenly on both feet.
```

### 6b. Mano levantada (adjuntar `administrativo-normal.png`)
```
Edit the attached image. Keep EXACTLY the same character: same face, wavy hair, mustache, gray suit, pale blue shirt, navy tie, green lanyard, coffee mug in the left hand, same body position, same size, same feet baseline, same 1024x1024 canvas and real transparent background. Change ONLY his right arm: raise it beside his head, elbow bent, open palm facing forward, as if politely asking for attention. Slightly raise his eyebrows. Nothing else moves. No shadow, no text, no background.
```

## 8. CALIDAD — mujer delgada de gafas y pelo café liso, blazer azul, tablet (agregado 2026-09-25)

Archivos: `calidad-normal.png` y `calidad-mano-levantada.png`. Especialidad: documental con cliente, interventorías y gerencia; inspecciones en campo; muy detallista.

### 8a. Normal
```
[BLOQUE DE ESTILO COMÚN]
Character: a slim Latina woman in her early thirties, straight medium-brown hair falling just past the shoulders with a neat side part, light olive skin, thin dark-rimmed rectangular glasses, sharp attentive eyes, composed professional half-smile that shows she notices every detail. Outfit: tailored navy blue (#183963) blazer over a white fitted top, slim brown trousers, low dark shoes, a green (#6ABF4B) lanyard with a badge, and a delicate thin gold wristwatch on her left wrist. NO earrings, NO necklace, no other jewelry. Holds a slim navy tablet in her left hand against her forearm, as if checking a document list; right arm relaxed DOWN at her side. Neutral idle pose, weight evenly on both feet, upright meticulous posture.
```

### 8b. Mano levantada (adjuntar `calidad-normal.png`)
```
Edit the attached image. Keep EXACTLY the same character: same face, glasses, straight brown hair, navy blazer, white top, brown trousers, green lanyard, delicate gold watch on the left wrist, tablet in the left hand, no earrings, no necklace, same body position, same size, same feet baseline, same 1024x1024 canvas and real transparent background. Change ONLY her right arm: raise it beside her head, elbow bent, open palm facing forward, as if politely asking for attention. Slightly raise her eyebrows. Nothing else moves. No shadow, no text, no background.
```

---

## 7. OFICINA VACÍA — `oficina-fondo.png` (adjuntar la imagen de referencia RenergeIA)

```
Edit the attached image of the RenergeIA Pixar-style office. REMOVE ALL SIX PEOPLE completely, leaving their chairs empty and their desks tidy (keep monitors, plants, mugs and papers). Keep everything else identical: the room, the large windows with the solar panel field outside, the wooden slat wall, the RENERGEIA logo on the wall, the whiteboard with "Trabaja Seguro, impulsa la calidad y cuida el medio ambiente", the green and navy ceiling lights, the plants, the blue-and-green carpet, the six desks with their chairs. Same camera angle, same lighting, same colors, same style. Landscape 16:9, 1920x1080, fully opaque. No people, no text other than the existing wall signage, no new furniture.
```

> Nota: para poder colocar cada personaje "sentado detrás de su escritorio", también necesitaremos en una segunda entrega los escritorios como piezas separadas. Primero generamos estas 13 y probamos la escena.
