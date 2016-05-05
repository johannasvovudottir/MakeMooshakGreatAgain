$(function () {
    console.debug("smuu");
    $('#solutionSubmit').hide();
    $('#solutionButton').click(function () {
        $('#Solution').val(editor.getValue())
        $('#solutionSubmit').click();
    });
});

$(function () {
    $("#tabs").tabs();
});

$(function() {
    $( "#DateCreated" ).datepicker();
    $( "#DueDate" ).datepicker();
});

$(function () {
    $('#formSubmit').click(function () {
        console.debug("Eg er her");
        $('#Description').val(editor.getValue())
    });
});


$(function () {
    $('#milestone').click(function(){
        var milestoneRow = $($('#milestoneTemplate').html());
    $('.milestoneTitle', milestoneRow).text($('#addMilestoneTitle').val());

    $('.addMilestoneTitle', milestoneRow).val($('#addMilestoneTitle').val()).attr('name', 'Milestones[' + counter + '].Title').attr('id', 'milestoneTitle' + counter);
    $('.addMilestoneWeight', milestoneRow).val($('#addMilestoneWeight').val()).attr('name', 'Milestones[' + counter + '].Weight').attr('id', 'milestoneWeight' + counter);
    $('.addMilestoneDescription', milestoneRow).val($('#addMilestoneDescription').val()).attr('name', 'Milestones[' + counter + '].Description').attr('id', 'milestoneDescription' + counter);
        
    $('#addMilestoneTitle').val('');
    $('#addMilestoneWeight').val('');
    $('#addMilestoneDescription').val('');
    $('#counter').val(++counter);
    $('#milestoneContainer').append(milestoneRow);
});
});
