﻿using System.IO;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;

namespace Bordercities
{
    
    public class Config
    {
        public EdgeDetection.EdgeDetectMode edgeMode;
        public bool edgeEnabled;
        public float sensNorm;
        public float sensDepth;
        public float edgeExpo;
        public float edgeSamp;
        public float edgeOnly;

        public bool firstTime = true;
        public KeyCode keyCode = KeyCode.LeftBracket;
        public KeyCode edgeToggleKeyCode;
        public bool subViewOnly;

        public bool autoEdge;

        public bool bloomEnabled;
        public float bloomThresh;
        public float bloomIntens;
        public float bloomBlurSize;

        public bool automaticMode;

        public Color currentColor;
        public float colorMultiplier;
        public float setR;
        public float setG;
        public float setB;

        public Color mixCurrentColor;
        public float mixColorMultiplier;
        public float mixSetR;
        public float mixSetG;
        public float mixSetB;

        public enum Tab
        {
            EdgeDetection = 0,
            Bloom = 1,
            Hotkey = 2,
        }


        public static void Serialize(string filename, Config config)
        {
            var serializer = new XmlSerializer(typeof(Config));

            using (var writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, config);
            }
        }

        public static Config Deserialize(string filename)
        {
            var serializer = new XmlSerializer(typeof(Config));

            try
            {
                using (var reader = new StreamReader(filename))
                {
                    var config = (Config)serializer.Deserialize(reader);
                    return config;
                }
            }
            catch { }
            return null;
        }

      
    }
}