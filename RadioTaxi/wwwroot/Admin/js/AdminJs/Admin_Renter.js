var Renter_vue = new Vue({
    el: "#Renter_vue",
    data:{
        dataDrivers: [],
        dataPackage: [],
        id: "",

        contactPersionCompany: "",
        contactPersionDriver: "",
        mobileDriver: "",
        mobileCompany: "",
        EmailCompany: "",
        EmailDriver: "",
        Status: "",
        

    
    },
    mounted() {
        this.loadCateItems();
    },
    methods: {
        formatDate(date) {
            const options = { year: 'numeric', month: '2-digit', day: '2-digit' };
            return date.toLocaleDateString('vi-VN', options);
        },
        loadCateItems() {
            $('#preloader').fadeIn();
            let currentPage = 0;
            if ($.fn.DataTable.isDataTable('#Renter_table')) {
                currentPage = $('#Renter_table').DataTable().page();
                $('#Renter_table').DataTable().destroy();
            }

            axios.get("/AdminRadio/AdminRenter/GetAll")
                .then((response) => {
                    this.dataDrivers = response.data;
                    $('#preloader').fadeOut();

                    return Promise.resolve();
                })
                .then(() => {
                    const table = $("#Renter_table").DataTable({
                        ...this.$globalConfig.createDataTableConfig(),
                        'columnDefs': [{
                            'targets': [-1],
                            'orderable': false,
                        }],
                        searching: true,
                        iDisplayLength: 7,
                        "ordering": false,
                        lengthChange: false,
                        aaSorting: [[0, "desc"]],
                        aLengthMenu: [
                            [5, 10, 25, 50, 100, -1],

                            ["5 dòng", "10 dòng", "25 dòng", "50 dòng", "100 dòng", "Tất cả"],
                        ]

                    });
                    if (currentPage !== 0) {
                        table.page(currentPage).draw('page');
                    }
                });
        },
      
     
        getItemsByIdDelete(id) {
            axios.get(`/AdminRadio/AdminRenter/GetByID/${id}`)
                .then((response) => {
                    this.id = response.data.id;
                    if (this.id != null) {
                        Swal.fire({
                            title: 'Delete product',
                            text: 'Are you sure you want to delete',
                            icon: 'warning',
                            showCancelButton: true,
                            confirmButtonText: 'Agree',
                            cancelButtonText: 'No!!!'
                        }).then((result) => {
                            if (result.isConfirmed) {
                                const formData = new FormData();
                                formData.append('ID', this.id);
                                axios.post('/AdminRadio/AdminRenter/Delete', formData, {
                                    headers: {
                                        'Content-Type': 'application/x-www-form-urlencoded'
                                    }
                                }).then(response => {
                                    Swal.fire({
                                        icon: 'success',
                                        title: 'Success',
                                        text: 'Success',
                                        confirmButtonText: 'OK',
                                    }).then((result) => {
                                        if (result.isConfirmed) {
                                            window.location.reload();


                                        }
                                    });

                                }).catch(error => {
                                    Swal.fire({
                                        icon: 'error',
                                        title: 'Error',
                                        text: 'An error occurred, please try again',
                                        confirmButtonText: 'OK'
                                    });
                                });
                            } else {
                                return;
                            }
                        });
                    }
                }).catch((error) => {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'An error occurred, please try again',
                        confirmButtonText: 'OK'
                    });
                })
        },
        async handleTransaction(items) {
            Swal.fire({
                title: 'Đang xử lý...',
                allowOutsideClick: false,
                onBeforeOpen: () => {
                    Swal.showLoading();
                },
                showConfirmButton: false
            });
            const formData = new FormData();
            try {
                if (!items.status) {

                    items.status = true;
                    formData.append('ID', items.id);

                    axios.post('/AdminRadio/AdminDrivers/HandlePayment', formData, {
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded'
                        }

                    }).then(res => {
                        if (res.data) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Success',
                                text: 'Success',
                                confirmButtonText: 'OK'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    window.location.reload();
                                }
                            });
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Error',
                                text: 'Error ',
                                confirmButtonText: 'OK'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    window.location.reload();
                                }
                            });
                        }
                    })

                }
            } catch {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Error ',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.reload();
                    }
                });
            }



        },
        async handlePayment(items) {
            Swal.fire({
                title: 'Đang xử lý...',
                allowOutsideClick: false,
                onBeforeOpen: () => {
                    Swal.showLoading();
                },
                showConfirmButton: false
            });
            const formData = new FormData();
            try {
                if (!items.payment) {

                    items.status = true;
                    formData.append('ID', items.id);

                    axios.post('/AdminRadio/AdminCompany/HandlePayment', formData, {
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded'
                        }

                    }).then(res => {
                        if (res.data) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Success',
                                text: 'Success',
                                confirmButtonText: 'OK'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    window.location.reload();
                                }
                            });
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Error',
                                text: 'Error trong quá trình gửi mail',
                                confirmButtonText: 'OK'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    window.location.reload();
                                }
                            });
                        }
                    })

                }
            } catch {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Error trong quá trình gửi mail',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.reload();
                    }
                });
            }



        },
    }
});