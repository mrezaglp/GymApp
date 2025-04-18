using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;

namespace GymApp.Infrastructure.Data
{
    public class AppSpecificationEvaluator : SpecificationEvaluator
    {
        public static AppSpecificationEvaluator Instance { get; } = new AppSpecificationEvaluator();
        public AppSpecificationEvaluator()
        {
            Evaluators.Add(DistinctEvaluator.Instance);
            Evaluators.Add(GroupByEvaluator.Instance);
        }
    }
}