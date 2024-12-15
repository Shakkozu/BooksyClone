using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksyClone.Contract.Availability;
public record CreateNewResourceRequest(
    Guid CorrelationId,
    Guid OwnerId
    );


public record GenerateNewLockRequest(
    Guid CorrelationId,
    Guid OwnerId,
    DateTime Start,
    DateTime End);

