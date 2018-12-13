function Deploy($Profile, $Skillname){
	ask clone -p $Profile
	$file1 = "./" + $Skillname + "/.ask/config"
	$file2 = "./.ask/config"
	$opts = @{ Path = $file1
	Destination = $file2  }
	Move-Item -Force @opts
	rm -Force -Recurse ./$Skillname
	ask deploy -p $Profile -t model
	git checkout ./.ask/config
}
cd ../Skill
Deploy -Profile andi -Skillname billa
cd ../Script