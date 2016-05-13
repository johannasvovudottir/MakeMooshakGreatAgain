$(function () {
    $('#solutionSubmit').hide();
    $('#solutionButton').click(function () {
        $('#Solution').val(editor.getValue())
        $('#solutionSubmit').click();
    });
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
            InitializeButtons();

            var formData = new FormData($(this)[0]);
            $.ajax({
                url: '/is/User/AddMilestone',
                data: formData,
                contentType: false,
                processData: false,
                method: 'POST',
                success: function (responseData) {
                    var html = '<input type="hidden" class="milestoneID addMilestoneFile" name = "Milestones[' + anotherCounter + '].TestCases" id = "milestoneFile' + anotherCounter + '" value = "' + responseData + '"/>'
                    $('#anotherCounter').val(++anotherCounter);
                    $('#addMilestoneFile').val('');
                    $('#milestoneContainer').append(html);
                },
                error: function (xhr, err) {
                    console.log("fokk everything");

                }
            })
            return false;
        });
});

$(function () {
    $('#formSubmit').click(function () {
        $('#Description').val(editor.getValue())
    });
});

$(document).ready(function () {
    InitializeButtons();
});

function InitializeButtons() {
    $('.removeMe').unbind('click').click(function () {
        $(this).closest('div.form-group').remove();
    });
}