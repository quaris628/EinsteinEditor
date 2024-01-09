using phi.graphics.drawables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static phi.io.KeyStroke;

namespace phi.graphics.renderables
{
    public class EditableText : Renderable
    {
        public const string UPPERCASE_ALPHABET_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string LOWERCASE_ALPHABET_CHARS = "abcdefghijklmnopqrstuvwxyz";
        public const string NUMBER_CHARS = "0123456789";


        protected Text text;
        protected readonly string allowedChars;
        public bool IsEditingEnabled { get; protected set; }
        protected bool isInit;
        private Dictionary<KeyStroke, Action> subscriptions;
        protected int anchorX { get; private set; }
        protected int anchorY { get; private set; }

        protected EditableText(EditableTextBuilder b)
        {
            this.text = b.text;
            this.allowedChars = b.allowedChars;
            this.IsEditingEnabled = b.isEditingEnabled;
            this.subscriptions = new Dictionary<KeyStroke, Action>();
            this.anchorX = b.anchorX;
            this.anchorY = b.anchorY;
        }

        public virtual void Initialize()
        {
            isInit = true;
            SubscribeChars();
            RecenterOnAnchor();
        }

        public virtual void Uninitialize()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            isInit = false;
            UnSubscribeChars();
            //IO.RENDERER.Remove(this); // commented out b/c I don't think this is necessary? But I haven't checked
        }

        protected void SubscribeChars()
        {
            foreach (char c in allowedChars)
            {
                foreach (KeyStroke keyStroke in CHAR_KEYSTROKE_MAP[c])
                {
                    Action a = () => { TypeChar(c); };
                    IO.KEYS.Subscribe(a, keyStroke);
                    subscriptions[keyStroke] = a;
                }
            }
            IO.KEYS.Subscribe(Backspace, Keys.Back);
        }

        protected void UnSubscribeChars()
        {
            foreach (char c in allowedChars)
            {
                foreach (KeyStroke keyStroke in CHAR_KEYSTROKE_MAP[c])
                {
                    IO.KEYS.Unsubscribe(subscriptions[keyStroke], keyStroke);
                    subscriptions.Remove(keyStroke);
                }
            }
            IO.KEYS.Unsubscribe(Backspace, Keys.Back);
        }

        public virtual void TypeChar(char c)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (!IsEditingEnabled) { return; }
            string newMessage = text.GetMessage() + c; // don't forget that ;)
            if (IsMessageValidWhileTyping(newMessage)) {
                text.SetMessage(newMessage);
                RecenterOnAnchor();
            }
        }

        public virtual void Backspace()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (!IsEditingEnabled || text.GetMessage().Length == 0) { return; }
            text.SetMessage(text.GetMessage().Substring(0, text.GetMessage().Length - 1));
            RecenterOnAnchor();
        }

        public virtual void Clear()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (!IsEditingEnabled) { return; }
            text.SetMessage("");
            RecenterOnAnchor();
        }

        protected virtual bool IsMessageValidWhileTyping(string message)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            return true;
        }
        protected virtual bool IsMessageValidAsFinalInternal(string message)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            return IsMessageValidWhileTyping(message);
        }
        public bool IsMessageValidAsFinal()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            return IsMessageValidAsFinalInternal(text.GetMessage());
        }

        public virtual void EnableEditing()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            IsEditingEnabled = true;
        }
        public virtual void DisableEditing() {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            IsEditingEnabled = false;
            RecenterOnAnchor();
        }

        public void SetAnchor(int x, int y)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            anchorX = x;
            anchorY = y;
            RecenterOnAnchor();
        }

        public virtual void RecenterOnAnchor()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            GetDrawable().SetXY(anchorX, anchorY);
        }

        public Drawable GetDrawable()
        {
            return text;
        }

        public static readonly Dictionary<char, KeyStroke[]> CHAR_KEYSTROKE_MAP = generateCharKeyMap();
        //public static readonly Dictionary<KeyStroke, char> KEYSTROKE_CHAR_MAP = generateKeyCharMap();

        private static Dictionary<char, KeyStroke[]> generateCharKeyMap()
        {
            Dictionary<char, KeyStroke[]> map = new Dictionary<char, KeyStroke[]>();

            // letters -- check shift or caps lock for uppercase vs lowercase
            for (int i = 0; i < UPPERCASE_ALPHABET_CHARS.Length; i++)
            {
                char upper = UPPERCASE_ALPHABET_CHARS[i];
                char lower = LOWERCASE_ALPHABET_CHARS[i];
                Keys key = (Keys)Enum.Parse(typeof(Keys), upper.ToString());
                KeyStroke shiftKeyStroke = new KeyStrokeBuilder(key).with(Keys.Shift).Build();
                KeyStroke noShiftKeyStroke = new KeyStroke(key);
                map.Add(upper, new KeyStroke[] { shiftKeyStroke });
                map.Add(lower, new KeyStroke[] { noShiftKeyStroke });
            }
            
            // numbers
            foreach (char c in NUMBER_CHARS)
            {
                Keys key1 = (Keys)Enum.Parse(typeof(Keys), "D" + c.ToString());
                Keys key2 = (Keys)Enum.Parse(typeof(Keys), "NumPad" + c.ToString());
                map.Add(c, new KeyStroke[] { new KeyStroke(key1), new KeyStroke(key2) });
            }

            // special characters
            map.Add(' ', new KeyStroke[] { new KeyStroke(Keys.Space) });
            map.Add('*', new KeyStroke[] { new KeyStroke(Keys.Multiply) });
            map.Add('+', new KeyStroke[] { new KeyStroke(Keys.Add), new KeyStroke(Keys.Oemplus) });
            map.Add('-', new KeyStroke[] { new KeyStroke(Keys.Subtract), new KeyStroke(Keys.OemMinus) });
            map.Add('.', new KeyStroke[] { new KeyStroke(Keys.Decimal), new KeyStroke(Keys.OemPeriod) });
            map.Add('/', new KeyStroke[] { new KeyStroke(Keys.Divide) });
            map.Add(';', new KeyStroke[] { new KeyStroke(Keys.OemSemicolon) });
            map.Add(',', new KeyStroke[] { new KeyStroke(Keys.Oemcomma) });
            map.Add('?', new KeyStroke[] { new KeyStroke(Keys.OemQuestion) });
            map.Add('~', new KeyStroke[] { new KeyStroke(Keys.Oemtilde) });
            map.Add('[', new KeyStroke[] { new KeyStroke(Keys.OemOpenBrackets) });
            map.Add(']', new KeyStroke[] { new KeyStroke(Keys.OemCloseBrackets) });
            map.Add('|', new KeyStroke[] { new KeyStroke(Keys.OemPipe) });
            map.Add('\'', new KeyStroke[] { new KeyStroke(Keys.OemQuotes) });
            map.Add('\\', new KeyStroke[] { new KeyStroke(Keys.OemBackslash) });

            // (not doing characters that would require a Shift + something for now)

            return map;
        }

        /*
        private static Dictionary<Keys, char> generateKeyCharMap()
        {
            Dictionary<Keys, char> map = new Dictionary<Keys, char>();

            // letters (just uppercase, lowercase can be added later if really needed)
            foreach (char c in ALPHABET_CHARS)
            {
            Keys key = (Keys)Enum.Parse(typeof(Keys), c.ToString());
            map.Add(key, c);
            }

            // numbers
            foreach (char c in NUMBER_CHARS)
            {
            Keys key1 = (Keys)Enum.Parse(typeof(Keys), "D" + c.ToString());
            Keys key2 = (Keys)Enum.Parse(typeof(Keys), "NumPad" + c.ToString());
            map.Add(key1, c);
            map.Add(key2, c);
            }

            // special characters
            map.Add(Keys.Space, ' ');
            map.Add(Keys.Multiply, '*');
            map.Add(Keys.Add, '+');
            map.Add(Keys.Oemplus, '+');
            map.Add(Keys.Subtract, '-');
            map.Add(Keys.OemMinus, '-');
            map.Add(Keys.Decimal, '.');
            map.Add(Keys.OemPeriod, '.');
            map.Add(Keys.Divide, '/');
            map.Add(Keys.OemSemicolon, ';');
            map.Add(Keys.Oemcomma, ',');
            map.Add(Keys.OemQuestion, '?');
            map.Add(Keys.Oemtilde, '~');
            map.Add(Keys.OemOpenBrackets, '[');
            map.Add(Keys.OemCloseBrackets, ']');
            map.Add(Keys.OemPipe, '|');
            map.Add(Keys.OemQuotes, '\'');
            map.Add(Keys.OemBackslash, '\\');

            // (not doing characters that would require a Shift + something for now)

            return map;
        }
        */

        public class EditableTextBuilder
        {
            public Text text { get; private set; }
            public string allowedChars { get; private set; }
            public bool isEditingEnabled { get; private set; }
            public int anchorX { get; private set; }
            public int anchorY { get; private set; }

            public EditableTextBuilder(Text text)
            {
                this.text = text;
                this.allowedChars = new string(CHAR_KEYSTROKE_MAP.Keys.ToArray()); // everything
                this.isEditingEnabled = true;
                this.anchorX = text.GetX();
                this.anchorY = text.GetY();
            }

            public EditableTextBuilder WithAllowedChars(string allowedChars) { this.allowedChars = allowedChars; return this; }
            public EditableTextBuilder WithEditingEnabled() { this.isEditingEnabled = true; return this; }
            public EditableTextBuilder WithEditingDisabled() { this.isEditingEnabled = false; return this; }
            public EditableTextBuilder WithAnchor(int x, int y) { this.anchorX = x; this.anchorY = y; return this; }

            public virtual EditableText Build() { return new EditableText(this); }
        }

    }
}
