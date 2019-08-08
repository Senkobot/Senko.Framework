using Senko.Framework;

namespace Senko.Localizations.PoEditor
{
    [Configuration("PoEditor")]
    public class PoEditorOptions
    {
        public string ApiToken { get; set; }

        public int ProjectId { get; set; }
    }
}
