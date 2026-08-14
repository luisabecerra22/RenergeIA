"use client";

export default function VideoInduccion() {
  return (
    <div style={{ margin: "1.5rem 0" }}>
      <h3 style={{ marginBottom: "0.75rem", color: "var(--azul-oscuro)" }}>
        Video de la capacitación
      </h3>
      <video
        controls
        controlsList="nodownload"
        onContextMenu={(e) => e.preventDefault()}
        style={{
          width: "100%",
          maxWidth: 720,
          borderRadius: 10,
          boxShadow: "0 2px 12px rgba(0,0,0,0.12)",
        }}
      >
        <source
          src="https://storage.googleapis.com/renergeia-media/video-induccion.mp4"
          type="video/mp4"
        />
        Tu navegador no soporta la reproducción de video.
      </video>
    </div>
  );
}
