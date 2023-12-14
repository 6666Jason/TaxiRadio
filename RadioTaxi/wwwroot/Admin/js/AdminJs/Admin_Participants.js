var participants_vue = new Vue({
    el: "#participants_vue",
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
            if ($.fn.DataTable.isDataTable('#participants_table')) {
                currentPage = $('#participants_table').DataTable().page();
                $('#participants_table').DataTable().destroy();
            }

            axios.get("/AdminRadio/AdminParticipants/GetAll")
                .then((response) => {
                    this.dataDrivers = response.data;
                    $('#preloader').fadeOut();

                    return Promise.resolve();
                })
                .then(() => {
                    const table = $("#participants_table").DataTable({
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
       
        getItemsById(id) {
            axios.get(`/AdminRadio/AdminParticipants/GetByID/${id}`)
                .then((response) => {
                    this.id = response.data.id;
                    this.contactPersionCompany = response.data.companyMain.contactPerson;
                    this.contactPersionDriver = response.data.driversMain.contactPerson;
                    this.mobileDriver = response.data.driversMain.mobile;
                    this.mobileCompany = response.data.companyMain.mobile;
                    this.EmailCompany = response.data.companyMain.email;
                    this.EmailDriver = response.data.driversMain.email;
                    this.Status = response.data.status;
                    
                    return Promise.resolve();
                });
        },
        
        resetData() {
            this.id = "";
            this.contactPersionCompany = "";
            this.contactPersionDriver = "";
            this.mobileDriver = "";
            this.mobileCompany = "";
            this.EmailCompany = "";
            this.EmailDriver = "";
            this.Status = "";
        },
     
        getItemsByIdDelete(id) {
            axios.get(`/AdminRadio/AdminParticipants/GetByID/${id}`)
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
                                axios.post('/AdminRadio/AdminParticipants/Delete', formData, {
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