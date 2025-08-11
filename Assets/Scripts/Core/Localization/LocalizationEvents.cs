using Core.Events;

namespace Core.Localization
{
    public struct LanguageChanged : IEvent
    {
        public LanguageId Id;
        public string Code;
        public LanguageChanged(LanguageId id, string code) { Id = id; Code = code; }
    }
}
