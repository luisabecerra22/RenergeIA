namespace RenergeIA.Web.Services.WhatsApp;

/// <summary>
/// Configuración del canal de WhatsApp (Cloud API de Meta).
/// Se lee de la sección "WhatsApp"; en Cloud Run se pasa como variables de entorno
/// WhatsApp__VerifyToken, WhatsApp__AppSecret, WhatsApp__AccessToken y WhatsApp__PhoneNumberId.
/// </summary>
public class WhatsAppOptions
{
    public const string Seccion = "WhatsApp";

    /// <summary>Token que inventamos nosotros; Meta lo devuelve al verificar el webhook.</summary>
    public string VerifyToken { get; set; } = "";

    /// <summary>App Secret de la app de Meta; con él se valida la firma de cada mensaje entrante.</summary>
    public string AppSecret { get; set; } = "";

    /// <summary>Token de acceso permanente (usuario del sistema) para enviar mensajes.</summary>
    public string AccessToken { get; set; } = "";

    /// <summary>Id del número de teléfono emisor, no el número en sí.</summary>
    public string PhoneNumberId { get; set; } = "";

    public string ApiVersion { get; set; } = "v23.0";

    /// <summary>Si es true, se rechazan los mensajes sin firma válida. Solo apagarlo para pruebas locales.</summary>
    public bool ValidarFirma { get; set; } = true;

    public bool PuedeEnviar => !string.IsNullOrWhiteSpace(AccessToken) && !string.IsNullOrWhiteSpace(PhoneNumberId);
    public bool PuedeVerificar => !string.IsNullOrWhiteSpace(VerifyToken);
}
