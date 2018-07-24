
function showCharts(nums, vals, Header, XValue, YValue) {
    
    var chart = Highcharts.chart('container', {
        chart: {
            type: 'column'
        },

        title: {
            text: ''
        },

        subtitle: {
            text: ''
        },

        legend: {
            align: 'right',
            verticalAlign: 'middle',
            layout: 'vertical'
        },

        xAxis: {
            categories: [""],
            labels: {
                x: -10
            }
        },

        yAxis: {
            allowDecimals: false,
            title: {
                text: YValue
            }
        },

        series: [{
            name: vals[0],
            data: [parseFloat(nums[0].trim())]
        }, {
            name: vals[1],
            data: [parseFloat(nums[1].trim())]
        }, {
            name: vals[2],
            data: [parseFloat(nums[2].trim())]
        }],

        responsive: {
            rules: [{
                condition: {
                    maxWidth: 500
                },
                chartOptions: {
                    legend: {
                        align: 'center',
                        verticalAlign: 'bottom',
                        layout: 'horizontal'
                    },
                    yAxis: {
                        labels: {
                            align: 'left',
                            x: 0,
                            y: -5
                        },
                        title: {
                            text: null
                        }
                    },
                    subtitle: {
                        text: null
                    },
                    credits: {
                        enabled: false
                    }
                }
            }]
        }
    });

}