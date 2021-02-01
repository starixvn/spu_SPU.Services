// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function site() {
    let btnCheckUpdateJobs = $("#btnCheckUpdateJobs");   

    $(document).ready(function () {        

        btnCheckUpdateJobs.on('click', (e) => {
            let result = confirm('Are you want check update?');

            if (!result) {
                e.preventDefault();
            }
        });

    });
}

$(site);