using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreManager.Models
{
    class _Grade
    {
        public int EXAM_ID { get; set; }
        public int TERM_ID { get; set; }
        public string STUDENT_ID { get; set; }
        public int SUBJECT_ID { get; set; }
        public double GRADE { get; set; }
    }
}
