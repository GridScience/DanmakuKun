﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DanmakuKun
{
    public static class CompletionListReader
    {

        public static void Read(string filename, IDictionary<string, IList<CompletionData>> dict)
        {
            using (XmlTextReader reader = new XmlTextReader(filename))
            {
                string listName;
                string listStatic;
                reader.ReadStartElement("lists");
                while (reader.IsStartElement("list"))
                {
                    listName = reader.GetAttribute("name");
                    listStatic = reader.GetAttribute("static");
                    if (!bool.Parse(listStatic))
                    {
                        listName = "$" + listName;
                    }
                    reader.ReadStartElement("list");
                    string name, type, returnType, description, source;
                    CompletionData data;
                    while (reader.IsStartElement("item"))
                    {
                        data = null;
                        name = reader.GetAttribute("name");
                        type = reader.GetAttribute("type");
                        returnType = reader.GetAttribute("return");
                        description = reader.GetAttribute("description");
                        source = reader.GetAttribute("source");
                        switch (type)
                        {
                            case "function":
                                if (string.IsNullOrEmpty(source))
                                {
                                    if (string.IsNullOrEmpty(description))
                                    {
                                        data = new FunctionCompletionData(name, returnType);
                                    }
                                    else
                                    {
                                        data = new FunctionCompletionData(name, returnType, description);
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(description))
                                    {
                                        data = new WithSourceFunctionCompletionData(name, returnType, source);
                                    }
                                    else
                                    {
                                        data = new WithSourceFunctionCompletionData(name, returnType, source, description);
                                    }
                                }
                                break;
                            case "property":
                                PropertyModifiers mod = PropertyModifiers.None;
                                string modifiers;
                                modifiers = reader.GetAttribute("modifiers");
                                if (!string.IsNullOrEmpty(modifiers))
                                {
                                    mod = (PropertyModifiers)Enum.Parse(typeof(PropertyModifiers), modifiers);
                                }
                                if (string.IsNullOrEmpty(source))
                                {
                                    if (string.IsNullOrEmpty(description))
                                    {
                                        data = new PropertyCompletionData(name, returnType, mod);
                                    }
                                    else
                                    {
                                        data = new PropertyCompletionData(name, returnType, mod, description);
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(description))
                                    {
                                        data = new WithSourcePropertyCompletionData(name, returnType, source, mod);
                                    }
                                    else
                                    {
                                        data = new WithSourcePropertyCompletionData(name, returnType, source, mod, description);
                                    }
                                }
                                break;
                        }
                        if (data != null)
                        {
                            IList<CompletionData> list;
                            dict.TryGetValue(listName, out list);
                            if (list == null)
                            {
                                list = new List<CompletionData>();
                                dict.Add(listName, list);
                            }
                            list.Add(data);
                        }
                        reader.ReadElementString();
                    }
                    reader.ReadEndElement();
                }
                reader.ReadEndElement();
            }
        }

    }
}