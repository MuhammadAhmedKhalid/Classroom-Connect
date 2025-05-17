function showDeleteConfirmation(deleteUrl, itemName, customTitle) {
    Swal.fire({
        title: customTitle !== null ? customTitle : `Are you sure you want to delete this ${itemName}?`,
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: customTitle !== null ? "Yes, remove!" : `Yes, delete the ${itemName}!`
    }).then((result) => {
        if (result.isConfirmed) {
            const antiForgeryToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
            fetch(deleteUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': antiForgeryToken
                }
            })
                .then(response => {
                    if (!response.ok) {
                        return response.json().then(errData => {
                            throw new Error(errData.message || `HTTP error! status: ${response.status}`);
                        });
                    }
                    return response.json();
                })
                .then(data => {
                    Swal.fire(
                        'Deleted!',
                        `The ${itemName} has been deleted.`,
                        'success'
                    ).then(() => {
                        if (data && data.redirectUrl) {
                            window.location.href = data.redirectUrl;
                        } else {
                            location.reload();
                        }
                    });
                })
                .catch((error) => {
                    console.error('Error deleting item:', error);
                    Swal.fire(
                        'Error!',
                        `There was a problem deleting the ${itemName}. ${error.message}`,
                        'error'
                    );
                });
        }
    });
}

$(document).on('click', '.delete-button', function () {
    const deleteUrl = $(this).data('url');
    const itemName = $(this).data('item-name') || 'item';
    const customTitle = $(this).data('title') || null;

    showDeleteConfirmation(deleteUrl, itemName, customTitle);
});
