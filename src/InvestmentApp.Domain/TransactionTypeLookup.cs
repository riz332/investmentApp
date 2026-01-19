using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestmentApp.Domain;

[Table("TransactionTypes")]
public class TransactionTypeLookup
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int TransactionTypeId { get; set; }

    public string Name { get; set; } = default!;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}