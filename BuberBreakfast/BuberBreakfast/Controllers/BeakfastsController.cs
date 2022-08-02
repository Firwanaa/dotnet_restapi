using BuberBreakfast.Contracts.Breakfast;
using BuberBreakfast.Models;
using BuberBreakfast.ServiceErrors;
using BuberBreakfast.Services.Breakfasts;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
namespace BuberBreakfast.Controllers;


public class BreakfastsController : ApiController
{
	private readonly IBreakfastService _breakfastService;

	public BreakfastsController(IBreakfastService breakfastService)
	{
		_breakfastService = breakfastService;
	}

	[HttpPost]
	public IActionResult CreateBreakfast(CreateBreakfastRequest request)
	{
		var breakfast = new Breakfast(
	    Guid.NewGuid(),
	    request.Name,
	    request.Description,
	    request.StartDateTime,
	    request.EndDateTime,
	    DateTime.UtcNow,
	    request.Savory,
	    request.Sweet
	);
		// TODO: save breakfast to database
		ErrorOr<Created> createdBreakfastResult = _breakfastService.CreateBreakfast(breakfast);

		// var response = new BreakfastResponse(
		//     breakfast.Id,
		//     breakfast.Name,
		//     breakfast.Description,
		//     breakfast.StartDateTime,
		//     breakfast.EndDateTime,
		//     breakfast.LastModifiedDateTime,
		//     breakfast.Savory,
		//     breakfast.Sweet
		// );
		return createdBreakfastResult.Match (
			created => CreatedAtGetBreakfast(breakfast),
			errors => Problem(errors)
		);
	}


	[HttpGet("{id:guid}")]
	public IActionResult GetBreakfast(Guid id)
	{
		ErrorOr<Breakfast> getBreakfastResult = _breakfastService.GetBreakfast(id);

		return getBreakfastResult.Match(
		    breakfast => Ok(MapBreakfastResponse(breakfast)),
		    errors => Problem(errors)
		);


	}



	[HttpPut("{id:guid}")]
	public IActionResult UpsertBreakfast(Guid id, UpsertBreakfastRequest request)
	{
		var breakfast = new Breakfast(
			id,
			request.Name,
		request.Description,
		request.StartDateTime,
		request.EndDateTime,
		DateTime.UtcNow,
		request.Savory,
		request.Sweet);

		ErrorOr<UpsertedBreakfast> upsertBreakfastResult = _breakfastService.UpdateBreakfast(breakfast);
		// TODO: return 201 if a new breakfast was created
		return upsertBreakfastResult.Match (
			upserted => upserted.IsNewlyCreated ? CreatedAtGetBreakfast(breakfast) : NoContent(),
			errors => Problem(errors)
		);
	}

	[HttpDelete("{id:guid}")]
	public IActionResult DeleteBreakfast(Guid id)
	{
		ErrorOr<Deleted> deleteBreakfastResult = _breakfastService.DeleteBreakfast(id);

		return deleteBreakfastResult.Match(
			deletedResult => NoContent(),
			errors => Problem(errors)

		);
	}


	private static BreakfastResponse MapBreakfastResponse(Breakfast breakfast)
	{
		return new BreakfastResponse(
			    breakfast.Id, breakfast.Name, breakfast.Description, breakfast.StartDateTime,
			    breakfast.EndDateTime,
			    breakfast.LastModifiedDateTime,
			    breakfast.Savory,
			    breakfast.Sweet
			);
	}
	private IActionResult CreatedAtGetBreakfast(Breakfast breakfast)
	{
	    return CreatedAtAction(
	    actionName: nameof(GetBreakfast),
	    routeValues: new { id = breakfast.Id },
	    value: MapBreakfastResponse(breakfast));
	}

}