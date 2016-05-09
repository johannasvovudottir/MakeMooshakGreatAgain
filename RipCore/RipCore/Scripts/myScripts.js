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
        $('#Description').val(editor.getValue())
    });
});

$(function () {
    $('#milestone').on('submit', function () {
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

        var formData = new FormData($(this)[0]);
        $.ajax({
        url: '/User/AddMilestone',
        data: formData,
        contentType: false,
        processData: false,
        method: 'POST',
        success: function (responseData) {
            console.log(responseData);
           // for (var i = 0; i < responseData.length; i++)
            //{
            console.log("smu");

                var html = '<input type="hidden" class="milestoneID addMilestoneFile" name = "Milestones[' + counter + '].TestCases" id = "milestoneFile' + counter +'" value = "' +responseData + '"/>'
                $('#milestoneContainer').append(html);
               // html += '<input type="hidden" ' + responseData[counter].Weight + '</p>';
               // <input type="hidden" id="milestoneAssignmentID" name="milestoneAssignmentID" value="@Model.ID" />
                //$('#milestoneContainer').append(html);
            //}
           // $('#reviewtext').val('');
        },
            error: function (xhr, err) {
            console.log("fokk everything");

        }

        })
        return false;
    });
});
