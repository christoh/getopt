using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace De.Hochstaetter.CommandLine.Models
{
    public class OptionDefinitionList : ObservableCollection<OptionDefinition>
    {
        public OptionDefinitionList()
        {
            CheckForDuplicates();
            base.CollectionChanged += OnCollectionChanged;
        }

        public OptionDefinitionList(IEnumerable<OptionDefinition> enumerable) : base(enumerable)
        {
            CheckForDuplicates();
            base.CollectionChanged += OnCollectionChanged;
        }

        public OptionDefinition this[string longName]
        {
            get
            {
                if (longName == null)
                {
                    throw new ArgumentNullException(null, "Index cannot be null");
                }

                if (longName.StartsWith("-"))
                {
                    throw new ArgumentException($"Index cannot start with '-'", nameof(longName));
                }

                return this.SingleOrDefault(d => d.LongName == longName);
            }
        }

        public OptionDefinition this[char shortName] =>
            shortName == default
                ? throw new ArgumentException("Index cannot be null char")
                : this.SingleOrDefault(d => d.ShortName == shortName);

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add && e.Action != NotifyCollectionChangedAction.Replace)
            {
                return;
            }

            CheckForDuplicates();
        }

        private void CheckForDuplicates()
        {
            var duplicates = string.Join(", ", GetDuplicates
            (
                this
                    .Where(d => d.LongName != null)
                    .Select(l => $"--{l.LongName}")
            )
            .Concat(GetDuplicates
            (
                this
                    .Where(d => d.ShortName != default)
                    .Select(d => $"-{d.ShortName}")
            )));

            if (duplicates != string.Empty)
            {
                throw new InvalidOperationException($"The following options are defined multiple times: {duplicates}");
            }
        }

        private static IEnumerable<string> GetDuplicates(IEnumerable<string> collection)
        {
            return
                collection
                    .GroupBy(l => l)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);
        }
    }

    public static class OptionDefinitionListExtensions
    {
        public static OptionDefinitionList ToOptionList(this IEnumerable<OptionDefinition> collection) => collection != null ? new OptionDefinitionList(collection) : new OptionDefinitionList();
    }
}
