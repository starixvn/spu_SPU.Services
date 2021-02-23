// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function site() {
    let domElements = {     
        tblJob: $("#tblJob")
    };

    $(document).ready(function () {       

        $('.toogle-job').on('click', function () {
            let span = $(this);
            let jobName = span.attr('data-job-name');

            if (span.hasClass('toogle-expand')) {
                span.removeClass('toogle-expand').addClass('toogle-collapse');
            } else {
                span.removeClass('toogle-collapse').addClass('toogle-expand');
            }

            domElements.tblJob.find('tr').each(function () {
                let tr = $(this);
                let jobNameCompare = tr.attr('data-job-name');

                if (jobName === jobNameCompare) {
                    if (tr.hasClass('is-hidden')) {
                        tr.removeClass('is-hidden');
                    } else {
                        tr.addClass('is-hidden');
                    }
                }
            });
        });
    });
}

$(site);