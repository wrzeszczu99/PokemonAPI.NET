﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewerController : Controller
    {

        private readonly IReviewerRepository _reviewerRepository;
        private readonly IMapper _mapper;

        public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        public IActionResult GetReviewers()
        {
            var reviewers = _mapper.Map<List<ReviewerDTO>>(_reviewerRepository.GetReviewers());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(reviewers);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewer(int id)
        {
            if (!_reviewerRepository.ReviewerExists(id))
                return NotFound();

            var reviewer = _mapper.Map<ReviewerDTO>(_reviewerRepository.GetReviewer(id));


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(reviewer);
        }

        [HttpGet("{id}/reviews")]
        public IActionResult GetReviewsByReviewer(int id)
        {
            if (!_reviewerRepository.ReviewerExists(id))
                return NotFound();

            var reviews = _mapper.Map<List<ReviewDTO>>( _reviewerRepository.GetReviewsByReviewer(id));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReviewer([FromBody] ReviewerDTO reviewerCreate)
        {
            if (reviewerCreate == null)
                return BadRequest(ModelState);

            var reviewer = _reviewerRepository.GetReviewers()
                .Where(r => r.LastName.Trim().ToUpper() == reviewerCreate.LastName.Trim().ToUpper())
                .FirstOrDefault();

            if (reviewer != null)
            {
                ModelState.AddModelError("", "Reviewer already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewerMap = _mapper.Map<Reviewer>(reviewerCreate);

            if (!_reviewerRepository.CreateReviewer(reviewerMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfuly created");
        }


        [HttpPut("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReviewer(int reviewerId, [FromBody] ReviewerDTO updatedReviewer)
        {
            if (updatedReviewer == null)
                return BadRequest(ModelState);
            if (reviewerId != updatedReviewer.Id)
                return BadRequest(ModelState);
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var reviewerMap = _mapper.Map<Reviewer>(updatedReviewer);

            if (!_reviewerRepository.UpdateReviewer(reviewerMap))
            {
                ModelState.AddModelError("", "Something went wrong updating reviewer");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{reviewerId")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReviewer(int reviewerId)
        {
            if (_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();

            var reviewerToDelete = _reviewerRepository.GetReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewerRepository.DeleteReviewer(reviewerToDelete))
                ModelState.AddModelError("", "Something went wrong deleting reviewer");

            return NoContent();
        }
    }
}
