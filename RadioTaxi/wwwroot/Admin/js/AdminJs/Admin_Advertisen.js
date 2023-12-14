var advertisen_vue = new Vue({
    el: "#advertisen_vue",
    data:{
        dataAdvertisen: [],
        dataPackage: [],
        companyName:"",
        designation:"",
        address:"",
        mobile:"",
        telephone:"",
        faxNumber:"",
        email:"",
        description:"",
        packageMainID: "",
        id: "",
        ckName: "",
        editor: "",

    
    },
    mounted() {
        this.loadCateItems();
        configureCKEditor('#editorMain', this, this.ckName);
        axios.get("/AdminRadio/AdminPage/GetAllPackage")
            .then((response) => {
                this.dataPackage = response.data;
                return Promise.resolve();
            });
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
            if ($.fn.DataTable.isDataTable('#adv_table')) {
                currentPage = $('#adv_table').DataTable().page();
                $('#adv_table').DataTable().destroy();
            }

            axios.get("/AdminRadio/AdminAdvertisen/GetAll")
                .then((response) => {
                    this.dataAdvertisen = response.data;
                    $('#preloader').fadeOut();

                    return Promise.resolve();
                })
                .then(() => {
                    const table = $("#adv_table").DataTable({
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
        async addItems() {
            try {

                const formData = new FormData();

                formData.append('CompanyName', this.companyName);
                formData.append('Designation', this.designation);
                formData.append('Address', this.address);
                formData.append('Mobile', this.mobile);
                formData.append('Telephone', this.telephone);
                formData.append('FaxNumber', this.faxNumber);
                formData.append('Email', this.email);
                formData.append('Description', this.ckName);
                formData.append('PackageId', this.packageMainID);

                await axios.post('/AdminRadio/AdminAdvertisen/Add', formData,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    });
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
            } catch (error) {
                console.error(error);
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Đã có Error xảy ra',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.reload();
                    }
                });
            }
        },
        getItemsById(id) {
            axios.get(`/AdminRadio/AdminAdvertisen/GetByID/${id}`)
                .then((response) => {
                    this.id = response.data.id;
                    this.companyName = response.data.companyName;
                    this.designation = response.data.designation;
                    this.address = response.data.address;
                    this.mobile = response.data.mobile;
                    this.telephone = response.data.telephone;
                    this.faxNumber = response.data.faxNumber;
                    this.email = response.data.email;
                    this.description = response.data.description;
                    this.ckName = response.data.description;
                    this.packageMainID = response.data.packageId;
                    this.initializeEditor()
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
            this.packageMainID = 0;
            this.companyName = "";
            this.designation = "";
            this.address = "";
            this.mobile = "";
            this.telephone = "";
            this.faxNumber = "";
            this.email = "";
            this.ckName = "";
            this.description = "";
            $('#Edit').on('hidden.bs.modal', () => {
                this.destroyEditor();
            });
        },
        async editItems() {
            try {


                const formData = new FormData();
                formData.append('CompanyName', this.companyName);
                formData.append('Designation', this.designation);
                formData.append('Address', this.address);
                formData.append('Mobile', this.mobile);
                formData.append('Telephone', this.telephone);
                formData.append('FaxNumber', this.faxNumber);
                formData.append('Email', this.email);
                formData.append('Description', this.ckName);
                formData.append('PackageId', this.packageMainID);
                formData.append('ID', this.id);
                await axios.post('/AdminRadio/AdminAdvertisen/Update', formData,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    });
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
            } catch (error) {
                console.error(error);
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Đã có Error xảy ra',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.reload();

                    }
                });
            }
        },
        getItemsByIdDelete(id) {
            axios.get(`/AdminRadio/AdminAdvertisen/GetByID/${id}`)
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
                                axios.post('/AdminRadio/AdminAdvertisen/Delete', formData, {
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
    }
});