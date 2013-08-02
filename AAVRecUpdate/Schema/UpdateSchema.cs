using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AAVRecUpdate.Schema
{
    class UpdateSchema
    {
        internal List<UpdateObject> AllUpdateObjects = new List<UpdateObject>();

        internal Schema.AAVRecUpdate AAVRecUpdate;
        internal Schema.AAVRecMainUpdate AAVRec;

        public UpdateSchema(XmlDocument xml)
        {
            foreach(XmlNode el in xml.DocumentElement.ChildNodes)
            {
                if ("AAVRecUpdate".Equals(el.Name))
                {
                    AAVRecUpdate = new Schema.AAVRecUpdate(el as XmlElement);
                    AllUpdateObjects.Add(AAVRecUpdate);
                }
                else if ("Update".Equals(el.Name))
                {
                    AAVRec = new Schema.AAVRecMainUpdate(el as XmlElement);
                    AllUpdateObjects.Add(AAVRec);
                }
                else if ("ModuleUpdate".Equals(el.Name))
                    AllUpdateObjects.Add(new Schema.ModuleUpdate(el as XmlElement));
            }
        }

        public bool NewUpdatesAvailable(string aavRecPath)
        {
            foreach (UpdateObject obj in AllUpdateObjects)
				if (obj.NewUpdatesAvailable(aavRecPath)) return true;

            return false;
        }
    }
}
