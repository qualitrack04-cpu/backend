
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CAPA updated)
    {
        var capa = await db.CAPAs.FindAsync(id);
        if (capa is null) return NotFound();
        capa.RootCause = updated.RootCause;
        capa.CorrectiveAction = updated.CorrectiveAction;
        capa.PreventiveAction = updated.PreventiveAction;
        capa.PicId = updated.PicId;
        capa.Deadline = updated.Deadline;
        await db.SaveChangesAsync();
        return NoContent();
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CAPA updated)
    {
        var capa = await db.CAPAs.FindAsync(id);
        if (capa is null) return NotFound();
        capa.RootCause = updated.RootCause;
        capa.CorrectiveAction = updated.CorrectiveAction;
        capa.PreventiveAction = updated.PreventiveAction;
        capa.PicId = updated.PicId;
        capa.Deadline = updated.Deadline;
        await db.SaveChangesAsync();
        return NoContent();
    }
}
