var feedback_vue = new Vue({
    el: "#feedback_vue",
    data:{
        dataDrivers: [],
        dataPackage: [],
        idDriver:"",
        contactPerson:"",
        address:"",
        mobile:"",
        telephone:"",
        experience:"",
        email:"",
        description:"",
        packageMainID: "",
        id: "",
        ckName: "",
        editor: "",
        City: "",

        Name:"",
        Mobile:"",
        Email:"",
        Type:"",
        Description:"",
        IDUser:"",
        CreateDate:"",
        UserName:"",

    
    },
    mounted() {
        this.loadCateItems();
       
      
    },
    watch: {
        ckName(newVal, oldVal) {
            if (!this.editor && newVal !== oldVal) {
                this.openEditor();
            }
        }
    },
    beforeDestroy() {
        if (this.editor) {
            this.editor.destroy();
        }
    },

    methods: {
        openEditor() {
           
            if (!this.editor) {
                configureCKEditor('#editor', this, this.ckName || {});
            }
        },
        formatDate(date) {
            const options = { year: 'numeric', month: '2-digit', day: '2-digit' };
            return date.toLocaleDateString('vi-VN', options);
        },
        formatCurrency(amount) {
            const formatter = new Intl.NumberFormat('en-US', {
                style: 'currency',
                currency: 'USD'
            });

            return formatter.format(amount);
        },
        loadCateItems() {
            $('#preloader').fadeIn();
            let currentPage = 0;
            if ($.fn.DataTable.isDataTable('#feedback_table')) {
                currentPage = $('#feedback_table').DataTable().page();
                $('#feedback_table').DataTable().destroy();
            }

            axios.get("/AdminRadio/AdminFeedback/GetAll")
                .then((response) => {
                    this.dataDrivers = response.data;
                    $('#preloader').fadeOut();

                    return Promise.resolve();
                })
                .then(() => {
                    const table = $("#feedback_table").DataTable({
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
            axios.get(`/AdminRadio/AdminFeedback/GetByID/${id}`)
                .then((response) => {
                    this.id = response.data.id;
                    this.Name = response.data.name;
                    this.Mobile = response.data.mobile;
                    this.Email = response.data.email;
                    this.Type = response.data.type;
                    this.Description = response.data.description;
                    this.IDUser = response.data.idUser;
                    this.UserName = response.data.applicationUserMain.userName;
                    
                    return Promise.resolve();
                });
        },
        destroyEditor() {
            if (this.editor) {
                this.editor.destroy();
                this.editor = null;
            }
        },
        initializeEditor() {
                this.destroyEditor(); 
                if (this.ckName == "") {
                    configureCKEditor('#editor', this, this.ckName);
                }
        },

        resetData() {
            this.id = "";
            this.Name = "";
            this.Mobile = "";
            this.Email = "";
            this.Type = "";
            this.Description = "";
            this.IDUser = "";
            this.UserName = "";
        },
     
        getItemsByIdDelete(id) {
            axios.get(`/AdminRadio/FeedBack/GetByID/${id}`)
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
                                axios.post('/AdminRadio/FeedBack/Delete', formData, {
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