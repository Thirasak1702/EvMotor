using EbikeRental.Application.DTOs;
using EbikeRental.Application.Interfaces;
using EbikeRental.Application.Interfaces.Repositories;
using EbikeRental.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EbikeRental.Web.Pages.Production.ProductionOrders;

[Authorize]
public class TrackModel : PageModel
{
    private readonly IProductionService _productionService;
    private readonly IRepository<ProductionOrderProcesses> _processRepository;
    private readonly IRepository<ProductionOrderQcs> _qcRepository;
    private readonly IQualityCheckService _qcService;

    public TrackModel(
        IProductionService productionService,
        IRepository<ProductionOrderProcesses> processRepository,
        IRepository<ProductionOrderQcs> qcRepository,
        IQualityCheckService qcService)
    {
        _productionService = productionService;
        _processRepository = processRepository;
        _qcRepository = qcRepository;
        _qcService = qcService;
    }

    public ProductionOrderDto Order { get; set; } = new();
    public List<ProductionOrderProcessesDto> Processes { get; set; } = new();
    public List<ProductionOrderQcsDto> QcSteps { get; set; } = new();
    public List<QualityCheckDto> QualityChecks { get; set; } = new();
    public int CurrentQcStep { get; set; } = 0;

    // Calculated values from QC
    public decimal CompletedQuantity { get; set; } = 0;
    public decimal ProgressPercentage { get; set; } = 0;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (!id.HasValue)
        {
            return RedirectToPage("./Index");
        }

        var result = await _productionService.GetOrderByIdAsync(id.Value);
        if (result.Success)
        {
            Order = result.Data;

            // Load Processes and QC Steps from Production Order
            Processes = Order.Processes;
            QcSteps = Order.QcSteps;

            // Load Quality Checks for this Production Order
            await LoadQualityChecks(id.Value);

            // Calculate Completed Quantity and Progress from QC
            CalculateCompletedFromQC();

            // Calculate Current QC Step based on QC status
            CalculateCurrentQcStep();

            return Page();
        }

        TempData["ErrorMessage"] = "Production order not found";
        return RedirectToPage("./Index");
    }

    private async Task LoadQualityChecks(int productionOrderId)
    {
        try
        {
            var qcResult = await _qcService.GetByProductionOrderIdAsync(productionOrderId);
            if (qcResult.Success && qcResult.Data != null)
            {
                QualityChecks = qcResult.Data;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading Quality Checks: {ex.Message}");
            QualityChecks = new List<QualityCheckDto>();
        }
    }

    private void CalculateCompletedFromQC()
    {
        // Calculate Completed Quantity from QC Items that have Passed status
        CompletedQuantity = 0;

        if (QualityChecks != null && QualityChecks.Any())
        {
            foreach (var qc in QualityChecks)
            {
                if (qc.Status == Domain.Enums.QualityCheckStatus.Passed && qc.Items != null)
                {
                    // Sum PassedQuantity from all items in Passed QC
                    CompletedQuantity += qc.Items.Sum(item => item.PassedQuantity);
                }
            }
        }

        // Calculate Progress Percentage
        if (Order.Quantity > 0)
        {
            ProgressPercentage = (CompletedQuantity / Order.Quantity) * 100;

            // Ensure it doesn't exceed 100%
            if (ProgressPercentage > 100)
            {
                ProgressPercentage = 100;
            }
        }
        else
        {
            ProgressPercentage = 0;
        }
    }

    public async Task<IActionResult> OnPostUpdateProcessAsync(int processId, string status, string? notes)
    {
        try
        {
            var process = await _processRepository.GetByIdAsync(processId);
            if (process == null)
            {
                TempData["ErrorMessage"] = "Process not found";
                return RedirectToPage(new { id = process?.ProductionOrderId });
            }

            process.Status = status;
            process.Notes = notes;

            if (status == "InProgress" && !process.StartedAt.HasValue)
            {
                process.StartedAt = DateTime.UtcNow;
            }
            else if (status == "Completed" && !process.CompletedAt.HasValue)
            {
                process.CompletedAt = DateTime.UtcNow;
            }

            await _processRepository.UpdateAsync(process);
            TempData["SuccessMessage"] = "Process updated successfully";

            return RedirectToPage(new { id = process.ProductionOrderId });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error updating process: {ex.Message}";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostUpdateQcAsync(int qcId, string status, string? notes, string? actualValues)
    {
        try
        {
            var qc = await _qcRepository.GetByIdAsync(qcId);
            if (qc == null)
            {
                TempData["ErrorMessage"] = "QC step not found";
                return RedirectToPage(new { id = qc?.ProductionOrderId });
            }

            qc.Status = status;
            qc.Notes = notes;
            qc.ActualValues = actualValues;

            if (status != "Pending" && !qc.CheckedAt.HasValue)
            {
                qc.CheckedAt = DateTime.UtcNow;
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                qc.CheckedByUserId = userId;
            }

            await _qcRepository.UpdateAsync(qc);
            TempData["SuccessMessage"] = "QC step updated successfully";

            return RedirectToPage(new { id = qc.ProductionOrderId });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error updating QC step: {ex.Message}";
            return Page();
        }
    }

    private void CalculateCurrentQcStep()
    {
        if (!QcSteps.Any())
        {
            CurrentQcStep = 0;
            return;
        }

        // Check actual QC status from database
        var passedSteps = QcSteps.Count(q => q.Status == "Passed");
        var inProgressStep = QcSteps.FirstOrDefault(q => q.Status == "InProgress");

        if (inProgressStep != null)
        {
            // Currently checking a specific step
            CurrentQcStep = inProgressStep.Sequence - 1;
        }
        else if (passedSteps == QcSteps.Count)
        {
            // All steps passed
            CurrentQcStep = QcSteps.Count;
        }
        else
        {
            // Use passed steps count
            CurrentQcStep = passedSteps;
        }
    }
}
