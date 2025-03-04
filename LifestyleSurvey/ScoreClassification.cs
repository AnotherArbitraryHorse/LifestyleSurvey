using System.Xml.Linq;

namespace LifestyleSurvey
{
    internal class ScoreClassification
    {
        private XElement definition;
        private Response parent;
        private string source; // Always "Age" at the moment.
        private int? lowerLimit;
        private int? upperLimit;
        public int   Score { get; }
        public ScoreClassification(XElement sc, Response response)
        {
            definition = sc;
            parent = response;
            source = definition.Attribute("source")!.Value;
            lowerLimit = Questionnaire.OptionalIntAttribute(definition, "GEQ");
            upperLimit = Questionnaire.OptionalIntAttribute(definition, "LEQ");
            Score = int.Parse(definition.Value.Trim());
        }

        /// <summary>
        /// Is this ScoreClassification applicable to the supplied source value?
        /// 
        /// It is iff. the value satisfies all supplied limits on its values
        /// (the optional lowerLimit and upperLimit values defined using GEQ/LEQ attributes).
        /// </summary>
        /// <param name="sourceValue">At the moment, the patient's age in years</param>
        /// <returns>whether the ScoreClassificatrion should be used</returns>
        internal bool ApplicableToSource(int sourceValue)
        {
            if (lowerLimit != null)
            {
                if (sourceValue < lowerLimit)
                {   // Below lower - not ok
                    return false;
                }
            }
            if (upperLimit != null)
            {
                if (sourceValue > upperLimit)
                {   // Above upper - not ok
                    return false;
                }
            }
            return true;
        }
    }
}