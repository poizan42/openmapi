#
# Script with default functions
#

@function openTxc () {
@	use org.openmapi.txc
@	logon \$1 \$2 \$3
@	openstore priv
@}

