//-----------------------------------------------------------------------
// <summary>
//      TestObject used for serialization and deserialization.
// </summary>
//-----------------------------------------------------------------------

using System;

using Newtonsoft.Json;

namespace Rest.Common.Tests
{
    //[DataContract]
    public class TestObject
    {
        [JsonProperty(Order = 1)]
        public string Name { get; set; }

        [JsonProperty(Order = 2)]
        public string Email { get; set; }

        [JsonProperty(Order = 3)]
        public string Notes { get; set; }

        [JsonProperty(Order = 4)]
        public int Ranking { get; set; }

    
        public bool Equals(TestObject t, StringComparison stringComparison)
        {
            if (Name.Equals(t.Name, stringComparison) && Email.Equals(t.Email, stringComparison) 
                && Notes.Equals(t.Notes, stringComparison) && Ranking.Equals(t.Ranking))
            {
                return true;
            }

            return false;
        }


 
    }



}
