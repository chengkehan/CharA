#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoryboardEditor
{
    public class EditorTextures
    {
        private static Texture2D _storyboard_start = null;
        public static Texture2D storyboard_start
        {
            get
            {
                if (_storyboard_start == null)
                {
                    _storyboard_start = Resources.Load<Texture2D>("storyboard_start");
                }
                return _storyboard_start;
            }
        }

        private static Texture2D _storyboard_error = null;
        public static Texture2D storyboard_error
        {
            get
            {
                if (_storyboard_error == null)
                {
                    _storyboard_error = Resources.Load<Texture2D>("storyboard_error");
                }
                return _storyboard_error;
            }
        }

        private static Texture2D _storyboard_break = null;
        public static Texture2D storyboard_break
        {
            get
            {
                if (_storyboard_break == null)
                {
                    _storyboard_break = Resources.Load<Texture2D>("storyboard_break");
                }
                return _storyboard_break;
            }
        }

        private static Texture2D _storyboard_warning = null;
        public static Texture2D storyboard_warning
        {
            get
            {
                if (_storyboard_warning == null)
                {
                    _storyboard_warning = Resources.Load<Texture2D>("storyboard_warning");
                }
                return _storyboard_warning;
            }
        }

        private static Texture2D _storyboard_talk = null;
        public static Texture2D storyboard_talk
        {
            get
            {
                if (_storyboard_talk == null)
                {
                    _storyboard_talk = Resources.Load<Texture2D>("storyboard_talk");
                }
                return _storyboard_talk;
            }
        }

        private static Texture2D _storyboard_choice = null;
        public static Texture2D storyboard_choice
        {
            get
            {
                if (_storyboard_choice == null)
                {
                    _storyboard_choice = Resources.Load<Texture2D>("storyboard_choice");
                }
                return _storyboard_choice;
            }
        }

        private static Texture2D _storyboard_end = null;
        public static Texture2D storyboard_end
        {
            get
            {
                if (_storyboard_end == null)
                {
                    _storyboard_end = Resources.Load<Texture2D>("storyboard_end");
                }
                return _storyboard_end;
            }
        }

        private static Texture2D _storyboard_link = null;
        public static Texture2D storyboard_link
        {
            get
            {
                if (_storyboard_link == null)
                {
                    _storyboard_link = Resources.Load<Texture2D>("storyboard_link");
                }
                return _storyboard_link;
            }
        }

        private static Texture2D _storyboard_trigger = null;
        public static Texture2D storyboard_trigger
        {
            get
            {
                if (_storyboard_trigger == null)
                {
                    _storyboard_trigger = Resources.Load<Texture2D>("storyboard_trigger");
                }
                return _storyboard_trigger;
            }
        }
    }
}
#endif
